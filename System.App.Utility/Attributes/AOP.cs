using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Activation;
using System.App.Utility.Attributes;
using System.Diagnostics;

// The class must inherit ContextBoundObject to use [PreProcess] and [PostProcess]
// http://www.codeproject.com/Articles/8436/Intercepting-method-calls-in-C-an-approach-to-AOSD
namespace System.App.Utility.Attributes
{
    #region interfaces
    
    public interface IPreProcessor
    {
        void Process(ref IMethodCallMessage msg);
    }

    public interface IPostProcessor
    {
        void Process(IMethodCallMessage callMsg, ref IMethodReturnMessage retMsg);
    }
    
    #endregion

    #region Attributes

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,
      AllowMultiple = true)]
    public class PreProcessAttribute : Attribute
    {
        private IPreProcessor p;
        public PreProcessAttribute(Type preProcessorType)
        {
            this.p = Activator.CreateInstance(preProcessorType) as IPreProcessor;
            if (this.p == null)
                throw new ArgumentException(String.Format("The type '{0}' " +
                  "does not implement interface IPreProcessor",
                  preProcessorType.Name, "processorType"));
        }

        public IPreProcessor Processor
        {
            get { return p; }
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class PostProcessAttribute : Attribute
    {
        private IPostProcessor p;
        public PostProcessAttribute(Type postProcessorType)
        {
            this.p = Activator.CreateInstance(postProcessorType) as IPostProcessor;
            if (this.p == null)
                throw new ArgumentException(String.Format("The type '{0}' " +
                  "does not implement interface IPostProcessor",
                  postProcessorType.Name, "processorType"));
        }

        public IPostProcessor Processor
        {
            get { return p; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InterceptAttribute : ContextAttribute
    {

        public InterceptAttribute()
            : base("Intercept")
        {
        }

        public override void Freeze(Context newContext)
        {
        }

        public override void
               GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            ctorMsg.ContextProperties.Add(new InterceptProperty());
        }

        public override bool IsContextOK(Context ctx,
                             IConstructionCallMessage ctorMsg)
        {
            InterceptProperty p = ctx.GetProperty("Intercept")
                                                 as InterceptProperty;
            if (p == null)
                return false;
            return true;
        }

        public override bool IsNewContextOK(Context newCtx)
        {
            InterceptProperty p = newCtx.GetProperty("Intercept")
                                                 as InterceptProperty;
            if (p == null)
                return false;
            return true;
        }

    }

    #endregion

    #region Messaging

    //IContextProperty, IContributeServerContextSink
	public class InterceptProperty : IContextProperty, IContributeObjectSink
	{
		public InterceptProperty() : base()
		{
		}
		#region IContextProperty Members

		public string Name
		{
			get
			{
				return "Intercept";
			}
		}

		public bool IsNewContextOK(Context newCtx)
		{
			InterceptProperty p = newCtx.GetProperty("Intercept") as InterceptProperty;
			if(p == null)
				return false;
			return true;
		}

		public void Freeze(Context newContext)
		{
		}

		#endregion

		#region IContributeObjectSink Members

		public System.Runtime.Remoting.Messaging.IMessageSink GetObjectSink(MarshalByRefObject obj, System.Runtime.Remoting.Messaging.IMessageSink nextSink)
		{
			return new InterceptSink(nextSink);
		}

		#endregion
	}

	public class InterceptSink : IMessageSink
	{
		private IMessageSink nextSink;

		public InterceptSink(IMessageSink nextSink)
		{
			this.nextSink = nextSink;
		}

		#region IMessageSink Members

		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMethodCallMessage mcm = (msg as IMethodCallMessage);
			this.PreProcess(ref mcm);
			IMessage rtnMsg = nextSink.SyncProcessMessage(msg);
			IMethodReturnMessage mrm = (rtnMsg as IMethodReturnMessage);
			this.PostProcess(msg as IMethodCallMessage,ref mrm);
			return mrm;
		}

		public IMessageSink NextSink
		{
			get
			{
				return this.nextSink;
			}
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			IMessageCtrl rtnMsgCtrl = nextSink.AsyncProcessMessage(msg,replySink);
			return rtnMsgCtrl;
		}

		#endregion

		private void PreProcess(ref IMethodCallMessage msg)
		{
			PreProcessAttribute[] attrs 
				= (PreProcessAttribute[])msg.MethodBase.GetCustomAttributes(typeof(PreProcessAttribute),true);
			for(int i=0;i<attrs.Length;i++)
				attrs[i].Processor.Process(ref msg);
		}

		private void PostProcess(IMethodCallMessage callMsg, ref IMethodReturnMessage rtnMsg)
		{
			PostProcessAttribute[] attrs 
				= (PostProcessAttribute[])callMsg.MethodBase.GetCustomAttributes(typeof(PostProcessAttribute),true);
			for(int i=0;i<attrs.Length;i++)
				attrs[i].Processor.Process(callMsg,ref rtnMsg);
			
		}
	}
#endregion

#region Implementation

    public class TracePreProcessor : IPreProcessor
    {
        #region IPreProcessor Members
        public void Process(ref IMethodCallMessage msg)
        {
            this.TraceMethod(msg.MethodName);
        }
        #endregion
        [Conditional("DEBUG")]
        private void TraceMethod(string method)
        {
            Trace.WriteLine(String.Format("PreProcessing:{0}", method));
        }
    }

    public abstract class ExceptionHandlingProcessor : IPostProcessor
    {
        public ExceptionHandlingProcessor()
        {
        }

        public void Process(IMethodCallMessage callMsg,
                      ref IMethodReturnMessage retMsg)
        {
            Exception e = retMsg.Exception;
            if (e != null)
            {
                this.HandleException(e);
                Exception newException = this.GetNewException(e);
                if (!object.ReferenceEquals(e, newException))
                    retMsg = new ReturnMessage(newException, callMsg);
            }
        }

        public abstract void HandleException(Exception e);
        public virtual Exception GetNewException(Exception oldException)
        {
            return oldException;
        }
    }

#endregion

}
