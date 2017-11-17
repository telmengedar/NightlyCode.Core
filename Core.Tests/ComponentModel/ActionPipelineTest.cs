using System.Threading;
using NightlyCode.Core.ComponentModel;
using NUnit.Framework;

namespace GM.Core.Tests.ComponentModel {

    [TestFixture]
    public class ActionPipelineTest {
         
        [Test]
        public void PipelineDoesntBlock() {
            ActionPipeline<object> pipeline = new ActionPipeline<object>(128);
            int processed = 0;
            pipeline.AddAction<object, object>((item) => item);
            pipeline.AddAction<object, object>(item => { 
                Thread.Sleep(5);
                return item;
            });
            pipeline.AddAction<object, object>(item => {
                Thread.Sleep(10);
                processed++;
                return null;
            });

            for(int i=0;i<256;++i)
                pipeline.Execute(new object());

            while(processed<256)
                Thread.Sleep(10);

            Assert.Pass("Pipeline did reach this point");
        }
    }
}