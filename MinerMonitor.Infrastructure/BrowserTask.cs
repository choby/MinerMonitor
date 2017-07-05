using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NSoup;
using MinerMonitor.Infrastructure.F2Pool;
using MinerMonitor.Infrastructure.Miner;

namespace MinerMonitor.Infrastructure
{
    public class BrowserTask
    {

        public BindingList<WorkerItem> list = new BindingList<WorkerItem>(); 
        
        /// <summary>
        /// 登录
        /// </summary>
        public async Task<bool> RestartAsync(bool dontRepeat = true)
        {

            MinerPage page = new MinerPage();
            return await page.RestartAsync();

        }

        /// <summary>
        /// 查询任务
        /// </summary>
        public async Task<BindingList<WorkerItem>> BrowserAsync()
        {
            list.Clear();


            F2PoolPage page = new F2PoolPage();
            var workerItems = await page.GetWorkerItemsAsync();
            list = new BindingList<WorkerItem>(workerItems.ToList());
            //var income = document.Body.GetElementsByClass("table-bordered")[0];//收益
            return list;
        }

    }
}
