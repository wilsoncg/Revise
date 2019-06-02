using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityValidation;

namespace UnityValidation
{
    public class ServiceRequestModelValidator :
        AbstractValidator<ServiceRequest>, IModelValidator
    {
        public ServiceRequestModelValidator()
        {
            RuleFor(r => r.Customer).NotNullCustomer();
        }
    }

    public class ServiceRequestBusinessRuleValidator :
        AbstractValidator<ServiceRequest>, IBusinessRuleValidator
    {
        public ServiceRequestBusinessRuleValidator()
        {
            RuleFor(r => r.Customer).SetValidator(new CustomerValidator());
        }
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must((c, _, context) =>
                {
                    var details = FromContext(context.ParentContext.RootContextData);
                    return details.Any();
                })
                .WithMessage("Customer details not found")
                .Must((c, _, context) =>
                {
                    var details = FromContext(context.ParentContext.RootContextData); ;
                    if (details.Any(d => d.Username.Contains("test")))
                        return false;
                    return true;
                })
                .WithMessage("Customer should not be test user");
        }

        public IList<CustomerDetails> FromContext(IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(nameof(CustomerDetails), out var value))
                        return (IList<CustomerDetails>)value;
            return Enumerable.Empty<CustomerDetails>().ToList();
        }
    }

    public interface IModelValidator { }
    public interface IBusinessRuleValidator { }
}

namespace FluentValidation
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, Property> NotNullCustomer<T, Property>(
            this IRuleBuilderInitial<T, Property> ruleBuilder) where Property : Customer
        {
            return
                ruleBuilder
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Customer must be provided")
                .Must(c =>
                {
                    var areIdsNonZero = c.CustomerAccountId > 0 && c.TradingAccountId > 0 && c.UserId > 0;
                    return areIdsNonZero;
                })
                .WithMessage(c => $"CustomerAccountId, TradingAccountId & UserId must be provided");
        }
    }
}
