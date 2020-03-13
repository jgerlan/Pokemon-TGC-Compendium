using Pokemon_TGC_Compendium.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_TGC_Compendium.ProducerConsumer
{
    class TGCWebScrapingProduceConsumer
    {
        private BlockingCollection<int> queueProduceToMount { get; set; }
        private BlockingCollection<int> queueMountToConsumer { get; set; }

        public void RunFlow(int nPages, bool chooseSN)
        {
            queueProduceToMount = new BlockingCollection<int>(3);
            queueMountToConsumer = new BlockingCollection<int>(3);

            Task Produce = Task.Run(() => {
                for (int i = 0; i < 5; i++)
                {
                    queueProduceToMount.Add(i);
                    Console.WriteLine("Add item {0} on queueProduceToMount.",i);
                }
            });

            Task Mount = Task.Run(() => {
                for (int i = 0; i < 5; i++)
                {
                    var item = queueProduceToMount.Take();
                    Console.WriteLine("Took out item {0} of queueProduceToMount.", item);
                    queueMountToConsumer.Add(item);
                    Console.WriteLine("Add item {0} on queueMountToConsumer", item);
                }
            });

            Task Consume = Task.Run(() => {
                for (int i = 0; i < 5; i++)
                {
                    var itemDois = queueMountToConsumer.Take();
                    Console.WriteLine("Took out item {0} of queueMountToConsumer.", itemDois);
                }
            });

            Task.WaitAll(Produce, Mount, Consume);
        }
    }
}
