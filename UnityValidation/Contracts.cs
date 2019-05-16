using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityValidation
{
    [ServiceContract(Namespace = "http://contoso/Service")]
    public interface IService
    {
        [OperationContract, ApplyValidation, ContextResolution]
        ServiceResponse GetSomething(ServiceRequest request);
    }

    /// <summary>
    /// Marker to indicate some business rule validation will happen on the server side for this class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BusinessRuleValidateAttribute : Attribute
    {
    }

    /// <summary>
    /// Marker to indicate some basic validation will happen on the server side for this class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelValidateAttribute : Attribute
    {
    }

    /// <summary>
    /// Marker to indicate some basic validation will happen on the server side for this method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApplyValidationAttribute : Attribute
    {
    }

    /// <summary>
    /// Marker to indicate paypal context resolution will happen on the server side for this method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextResolutionAttribute : Attribute
    {
    }

    public enum ResponseCode
    {
        Error = 0,
        Ok = 1
    }

    [DataContract]
    public class ServiceResponse
    {
        [DataMember, EnumMember]
        public ResponseCode Code;

        [DataMember]
        public string Description;
    }

    [DataContract]
    public class Customer
    {
        [DataMember]
        public int CustomerAccountId;
        [DataMember]
        public int TradingAccountId;
        [DataMember]
        public int UserId;
    }

    [DataContract, ModelValidate, BusinessRuleValidate]
    public class ServiceRequest
    {
        [DataMember]
        public Customer Customer { get; set; }
    }
}
