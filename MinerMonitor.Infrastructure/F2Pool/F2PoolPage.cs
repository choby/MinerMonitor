using NSoup;
using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinerMonitor.Infrastructure.F2Pool
{
    public class F2PoolPage
    {
        string poolUrl;
        Document document;
        public F2PoolPage()
        {
            poolUrl = ConfigurationManager.AppSettings["poolUrl"].ToString();
        }

        public F2PoolPage(string poolUrl)
        {
            this.poolUrl = poolUrl;
        }


        public async Task<IEnumerable<WorkerItem>> GetWorkerItemsAsync()
        {
            if (document == null)
            {
                await Task.Run(()=> {
                    loadPageAsync();
                });
            }
            IList<WorkerItem> list = new List<WorkerItem>();

            var workers = document.Body.GetElementById("workers");//矿工
            var trs = workers.GetElementsByTag("tbody")[0].GetElementsByTag("tr");
            foreach (var tr in trs)
            {
                var tds = tr.GetElementsByTag("td");
                WorkerItem item = new WorkerItem();

                item.WorkerName = tds[0].Text();
                item.SpeedPer15Minutes = tds[1].Text();
                item.SpeedPerHour = tds[2].Text();
                item.SpeedPer24Hours = tds[4].Text();

                var timeScript = tds[6].GetElementsByTag("script")[0]
                    .Html()
                    .Replace("document.write(formatTimestampUptoMinute(new Date(", "")
                    .Replace(")))", "");

                long unixTimeStamp = (long)decimal.Parse(timeScript);

                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                DateTime dt = startTime.AddSeconds(unixTimeStamp);
                item.LastSubmitTime = dt;
                list.Add(item);
            }

            return list;
        }

        void loadPageAsync()
        {
            Robot _robot = new Robot();

            string pageContent = null;

           
            pageContent = _robot.GetPage(poolUrl, Encoding.UTF8);

            while (string.IsNullOrEmpty(pageContent))
            {
                Thread.Sleep(2000);
                pageContent = _robot.GetPage(poolUrl, Encoding.UTF8);

            }
            this.document = NSoupClient.Parse(pageContent);
        }
    }
}
