using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EasyNews.NewsService;

namespace EasyNews
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
                
        protected void Page_Load(object sender, EventArgs e)
        {                        
            try
            {
                int numberOfNew = 0;
                NewsHandlerClient newsHandlerClient = new NewsHandlerClient();                
                int newsCount = newsHandlerClient.NewsCount();
                if (newsCount == 0)
                {
                    HyperLink1.Attributes.Add("class", "hidden");
                    HyperLink2.Attributes.Add("class", "hidden");
                    return;
                }

                if (!Request.QueryString.HasKeys())
                {
                    HyperLink1.NavigateUrl = "#";
                    HyperLink1.Attributes.Add("class", "hidden");
                    HyperLink2.NavigateUrl = "?new=" + (numberOfNew + 1);
                }
                else
                {
                    numberOfNew = int.Parse(Request.QueryString["new"]);
                    if (Request.QueryString["save"] == "1") newsHandlerClient.SaveNewByNumber(numberOfNew);

                    if (numberOfNew == 0)
                    {
                        HyperLink1.NavigateUrl = "#";
                        HyperLink1.Attributes.Add("class", "hidden");
                        HyperLink2.NavigateUrl = "?new=" + (numberOfNew + 1);
                    }
                    else if (numberOfNew == (newsCount - 1))
                    {
                        HyperLink1.NavigateUrl = "?new=" + (numberOfNew - 1);
                        HyperLink2.NavigateUrl = "#";
                        HyperLink2.Attributes.Add("class", "hidden");
                    }
                    else
                    {                       
                        HyperLink1.NavigateUrl = "?new=" + (numberOfNew - 1);
                        HyperLink2.NavigateUrl = "?new=" + (numberOfNew + 1);
                    }
                    
                }

                if(newsCount == 1) HyperLink2.Attributes.Add("class", "hidden");

                New item;

                if(numberOfNew < newsCount)
                    item = newsHandlerClient.GetNewByNumber(numberOfNew);
                else
                    item = null;
                
                if (item != null)
                {
                    Label3.Text = (DateTime.ParseExact(item.PubDate, "ddd, dd MMM yyyy HH:mm:ss zzzz", System.Globalization.CultureInfo.InvariantCulture)).ToLongDateString();
                    Label2.Text = item.Title;
                    Image1.ImageUrl = item.Enclosure.Url;
                    TextBox1.InnerText = item.Description;
                }
                else
                {
                    Label2.Text = "Новость №" + numberOfNew + " не найдена";
                    return;
                }

                New[] savedNews = newsHandlerClient.GetNewsFromTable();
                bool flag = false;
                foreach(New now in savedNews)
                {
                    if (item.Title == now.Title) flag = true;
                    BulletedList1.Items.Add(new ListItem(now.Title)); 
                }
                if (flag)
                {
                    HyperLink4.Attributes.Add("class", "saved_button");
                    HyperLink4.NavigateUrl = "#";
                    HyperLink4.Text = "Сохранено";
                }
                else
                {
                    HyperLink4.NavigateUrl = "?save=1&new=" + (numberOfNew);
                }
            }
            catch (Exception ex)
            {
                Label2.Text = ex.Message;
            }

        }        
    }
}
