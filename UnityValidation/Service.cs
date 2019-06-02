using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using FluentValidation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using FluentValidation.Results;
using AutoMapper;

namespace UnityValidation
{
    public class Service : IService
    {
        public ServiceResponse GetSomething(ServiceRequest request)
        {
            return new ServiceResponse();
        }
    }

    // Mark as Serializable to test runner can marshal across app domains
    // https://github.com/Microsoft/vstest/issues/1441
    [Serializable]
    public class CustomerDetails { public string Username; }

    public interface IGetFromDatabase
    {
        IEnumerable<CustomerDetails> CustomerDetails(Customer customer);
    }

    public interface IContext
    {
        IEnumerable<CustomerDetails> CustomerDetails { get; }
        ContextData LoadForCustomer(Customer customer);
    }

    public class Context : IContext
    {
        IEnumerable<CustomerDetails> _details;
        IGetFromDatabase _getFromDatabase;

        public Context(IGetFromDatabase getFromDatabase)
        {
            _getFromDatabase = getFromDatabase;
        }

        public IEnumerable<CustomerDetails> CustomerDetails => GetCustomerDetails();

        public ContextData LoadForCustomer(Customer customer)
        {
            return new ContextData(_getFromDatabase.CustomerDetails(customer));
        }

        IEnumerable<CustomerDetails> GetCustomerDetails()
        {
            var context = GetFromContext();
            if (context != null)
                _details = context.CustomerDetails;

            return _details;
        }

        ContextData GetFromContext()
        {
            return CallContext.LogicalGetData(nameof(ContextData)) as ContextData;
        }
    }

    // Mark as Serializable to test runner can marshal across app domains
    // https://github.com/Microsoft/vstest/issues/1441
    [Serializable]
    public class ContextData
    {
        IEnumerable<CustomerDetails> _details = Enumerable.Empty<CustomerDetails>();
        public IEnumerable<CustomerDetails> CustomerDetails => _details;

        public ContextData(IEnumerable<CustomerDetails> details)
        {
            if (details != null & details.Any())
                _details = details;
        }
    }

    public class UnityInitializer
    {
        public IUnityContainer Initialize(IUnityContainer container)
        {
            Mapper.AddProfile(new MappingProfile());

            container.RegisterType<IGetFromDatabase>(
                new InjectionFactory(c => new PretendGetFromDatabase()));
            container.RegisterType<IContext>(
                new InjectionFactory(c => new Context(c.Resolve<IGetFromDatabase>())));

            container
                .RegisterType<IModelValidator, ServiceRequestModelValidator>
                (nameof(ServiceRequestModelValidator));
            container
                .RegisterType<IBusinessRuleValidator, ServiceRequestBusinessRuleValidator>
                (nameof(ServiceRequestBusinessRuleValidator));

            container
                .AddNewExtension<Interception>()
                .RegisterType<ModelValidationBehavior>(
                    new InjectionFactory(c => { return new ModelValidationBehavior(c); }))
                .RegisterType<IService>(
                    new ContainerControlledLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ModelValidationBehavior>());
            container
                .AddNewExtension<Interception>()
                .RegisterType<ContextResolutionBehaviour>(
                    new InjectionFactory(c => { return new ContextResolutionBehaviour(c); }))
                .RegisterType<IService>(
                    new ContainerControlledLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ContextResolutionBehaviour>());
            container
                .AddNewExtension<Interception>()
                .RegisterType<BusinessRulesValidationBehaviour>(
                    new InjectionFactory(c => { return new BusinessRulesValidationBehaviour(c); }))
                .RegisterType<IService>(
                    new ContainerControlledLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<BusinessRulesValidationBehaviour>());

            container.RegisterType<IService>(
                new InjectionFactory(c => { return new Service(); }));

            return container;
        }
    }

    static internal class Reflection
    {
        public static bool IsMethodMarkedWithAttribute<TAttribute>(
            IEnumerable<Attribute> attributes)
        {
            return attributes.Any(x => x.GetType() == typeof(TAttribute));
        }

        public static IEnumerable<ParameterInfo> ParametersMarkedForValidation<T>(
            ParameterInfo[] parameters)
        {
            var r =
                parameters
                .Select(x => 
                    new {
                        attributes = x.ParameterType.GetCustomAttributes(),
                        parameter = x
                    });
            var f =
                r
                .Where(x => x.attributes.Any(y => y.GetType() == typeof(T)))
                .Select(x => x.parameter);
            return f;
        }

        public static IEnumerable<T> ParametersWithMemberType<T>(
            ParameterInfo[] parameters, object obj) where T : class
        {
            var matchingProperties =
                parameters
                .SelectMany(x => x.ParameterType.GetTypeInfo().DeclaredProperties)
                .Where(x => x.PropertyType == typeof(T));

            if (!matchingProperties.Any())
                return Enumerable.Empty<T>();

            var result =
                matchingProperties.Select(x =>
                {
                    var property =
                      obj.GetType().GetProperty(x.Name);
                    return (T)property.GetValue(obj, null);
                });
            return result;
        }
    }

    public class ModelValidationBehavior : IInterceptionBehavior
    {
        TextWriterTraceListener _log = new TextWriterTraceListener();
        readonly IUnityContainer _container;

        public ModelValidationBehavior(IUnityContainer container)
        {
            _container = container;
        }

        public bool WillExecute => true;

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(
            IMethodInvocation input,
            GetNextInterceptionBehaviorDelegate getNext)
        {
            var methodBase = input.MethodBase;
            if (Reflection.IsMethodMarkedWithAttribute<ApplyValidationAttribute>(
                methodBase.GetCustomAttributes()))
            {
                var paramInfo = 
                    Reflection
                    .ParametersMarkedForValidation<ModelValidateAttribute>(
                        methodBase.GetParameters());
                if (!paramInfo.Any())
                {
                    _log.WriteLine($"No parameters marked with {nameof(ModelValidateAttribute)}");
                    return getNext()(input, getNext);
                }

                var p = paramInfo.First();
                var name = $"UnityValidation.{p.ParameterType.Name}ModelValidator";
                var validators =
                    _container
                    .ResolveAll<IModelValidator>()
                    .Where(x => x.GetType().FullName == name);

                if (!validators.Any())
                {
                    _log.WriteLine($"No {nameof(IModelValidator)} found with name {name}");
                    return getNext()(input, getNext);
                }

                var validator = validators.First();
                var returnType = ((MethodInfo)methodBase).ReturnType;
                if (validator is IValidator)
                {
                    var validationResult = ((IValidator)validator).Validate(input.Arguments[0]);
                    if (!validationResult.IsValid)
                    {
                        _log.WriteLine(
                            $"{p.ParameterType.Name}ModelValidator rejected request " +
                            $"with reason, {validationResult.ToString()} ");
                        return
                            input.CreateMethodReturn(
                                Mapper.Map(
                                    validationResult, 
                                    typeof(ValidationResult), 
                                    returnType));
                    }
                }
            }

            return getNext()(input, getNext);
        }
    }

    public class ContextResolutionBehaviour : IInterceptionBehavior
    {
        readonly IUnityContainer _container;

        public ContextResolutionBehaviour(IUnityContainer container)
        {
            _container = container;
        }

        public bool WillExecute => true;

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(
            IMethodInvocation input,
            GetNextInterceptionBehaviorDelegate getNext)
        {
            if (!Reflection.IsMethodMarkedWithAttribute<ContextResolutionAttribute>(input.MethodBase.GetCustomAttributes()))
                return getNext()(input, getNext);

            if (input.Arguments.Count != 1)
                return getNext()(input, getNext);

            var matchingParameters =
                Reflection.ParametersWithMemberType<Customer>(
                    input.MethodBase.GetParameters(),
                    input.Arguments[0]);
            if (!matchingParameters.Any())
                return getNext()(input, getNext);

            var context =
                _container
                .Resolve<IContext>()
                .LoadForCustomer(matchingParameters.First());
            CallContext.LogicalSetData(nameof(ContextData), context);

            return getNext()(input, getNext);
        }
    }

    public class BusinessRulesValidationBehaviour : IInterceptionBehavior
    {
        TextWriterTraceListener _log = new TextWriterTraceListener();
        readonly IUnityContainer _container;

        public BusinessRulesValidationBehaviour(IUnityContainer container)
        {
            _container = container;
        }

        public bool WillExecute => true;

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(
            IMethodInvocation input,
            GetNextInterceptionBehaviorDelegate getNext)
        {
            var attributes = input.MethodBase.GetCustomAttributes();
            var isMethodApplicable =
                Reflection.IsMethodMarkedWithAttribute<ContextResolutionAttribute>(attributes) &
                Reflection.IsMethodMarkedWithAttribute<ApplyValidationAttribute>(attributes);

            if (!isMethodApplicable)
                return getNext()(input, getNext);

            var paramInfo = Reflection.ParametersMarkedForValidation<BusinessRuleValidateAttribute>(input.MethodBase.GetParameters());
            if (!paramInfo.Any())
            {
                _log.WriteLine($"No parameters marked with {nameof(BusinessRuleValidateAttribute)}");
                return getNext()(input, getNext);
            }

            var p = paramInfo.First();
            var name = $"UnityValidation.{p.ParameterType.Name}BusinessRuleValidator";
            var validators =
                _container
                .ResolveAll<IBusinessRuleValidator>()
                .Where(x => x.GetType().FullName == name);

            if (!validators.Any())
            {
                _log.WriteLine($"No {nameof(IBusinessRuleValidator)} found with name {name}");
                return getNext()(input, getNext);
            }

            var validator = validators.First();
            var returnType = ((MethodInfo)input.MethodBase).ReturnType;
            var request = input.Arguments[0];
            if (validator is IValidator)
            {
                var validationResult = ((IValidator)validator).Validate(LinkToValidationContext(request));
                if (!validationResult.IsValid)
                {
                    _log.WriteLine(
                        $"{p.ParameterType.Name}Validator rejected request{WhichCustomer()}, " +
                        $"with reason {validationResult.ToString()}");
                    return
                        input.CreateMethodReturn(
                            Mapper.Map(validationResult, typeof(ValidationResult), returnType));
                }
            }

            return getNext()(input, getNext);
        }

        string WhichCustomer()
        {
            var details =
                _container
                .Resolve<IContext>()
                .CustomerDetails;
            if (!details.Any())
                return string.Empty;

            var username = details.First().Username;

            return $" for Username: {username}";
        }

        ValidationContext LinkToValidationContext(object request)
        {
            var context =
                _container
                .Resolve<IContext>();

            var vc = new ValidationContext(request);
            vc.RootContextData[nameof(CustomerDetails)] = context.CustomerDetails;
            return vc;
        }
    }
}
