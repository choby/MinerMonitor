using NSoup;
using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace MinerMonitor.Infrastructure.Miner
{
    public class MinerPage
    {
        string resartUrl;
        Robot _robot = new Robot();
        Document document;
        public MinerPage()
        {

            resartUrl = ConfigurationManager.AppSettings["resartUrl"].ToString(); 
        }

        public MinerPage(string resartUrl)
        {
            this.resartUrl = resartUrl;
        }

        public async Task<bool> RestartAsync()
        {
            return await RestartWithStaticIPAsync();
        }

        private async Task<bool> RestartWithStaticIPAsync()
        {
            if (document == null)
            {
                await Task.Run(() => {
                    loadPageAsync();
                });
            }

            var inputs = document.GetElementsByTag("form")[0]
                                 .GetElementsByTag("input");

            string ip, netmask, gateway, dns;
            ip = inputs.Where(i => i.Attributes["name"] == "ip").Select(i => i.Val()).FirstOrDefault();
            netmask = inputs.Where(i => i.Attributes["name"] == "netmask").Select(i => i.Val()).FirstOrDefault();
            gateway = inputs.Where(i => i.Attributes["name"] == "gateway").Select(i => i.Val()).FirstOrDefault();
            dns = inputs.Where(i => i.Attributes["name"] == "dns").Select(i => i.Val()).FirstOrDefault();

            
            string postData = $"ip={ip}&netmask={netmask}&gateway={gateway}&dns={dns}&static=设置为固定IP";

            string result = this.postPage(postData);

            if (result.Contains("网络设置完成, 系统将重启 ! 请等待 ..."))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void loadPageAsync()
        {
            

            string pageContent = null;


            pageContent = _robot.GetPage(resartUrl, Encoding.UTF8);

            while (string.IsNullOrEmpty(pageContent))
            {
                Thread.Sleep(2000);
                pageContent = _robot.GetPage(resartUrl, Encoding.UTF8);

            }
            this.document = NSoupClient.Parse(pageContent);
        }


        private string postPage(string postData)
        {
            var response = _robot.PostPage(resartUrl, postData, null, Encoding.UTF8);
            while (string.IsNullOrEmpty(response))
            {
                Thread.Sleep(2000);
                response = _robot.PostPage(resartUrl, postData, null, Encoding.UTF8);

            }
            return response;
        }
    }
}
