using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Order : BaseModel
    {

        [DisplayName("Mã hóa đơn mua hàng")]
        public int OrderID { get; set; }

        #region Order Specifications

        [DisplayName("Tổng tiền")]
        public double Total { get; set; }

        [DisplayName("Ngày mua")]
        public DateTime Date { get; set; }

        [DisplayName("Đã in?")]
        public bool IsPrinted { get; set; }
        #endregion

        //#region ApplicationUser ForeignKey (Staff in system)
        //public string ApplicationUserID { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }
        //#endregion

        #region Customer ForeignKey 

        [DisplayName("Khách mua hàng")]
        public int CustomerID { get; set; }

        [DisplayName("Khách mua hàng")]
        public virtual Customer Customer { get; set; }
        #endregion

        #region Navigator OrderDetails

        [DisplayName("Danh sách chi tiết hóa đơn")]
        public IList<OrderDetail> OrderDetails { get; set; }
        #endregion
        private String readGroup(String group)
        {
            String[] readDigit = { " Không", " Một", " Hai", " Ba", " Bốn", " Năm", " Sáu", " Bảy", " Tám", " Chín" };
            String temp = "";
            if (group == "000") return "";
            //read number hundreds
            temp = readDigit[Int32.Parse(group.Substring(0, 1))] + " Trăm";
            //read number tens
            if (group.Substring(1, 1).Equals("0"))
                if (group.Substring(2, 1).Equals("0")) return temp;
                else
                {
                    temp += " Lẻ" + readDigit[Int32.Parse(group.Substring(2, 1))];
                    return temp;
                }
            else
                temp += readDigit[Int32.Parse(group.Substring(1, 1))] + " Mươi";
            //read number

            if (group.Substring(2, 1) == "5") temp += " Lăm";
            else if (group.Substring(2, 1) != "0") temp += readDigit[Int32.Parse(group.Substring(2, 1))];
            return temp;
        }

        public String ReadMoney()
        {
            String num = Total.ToString().Trim();
            if ((num == null) || (num.Equals(""))) return "";
            String temp = "";

            //length <= 18
            while (num.Length < 18)
            {
                num = "0" + num;
            }

            String g1 = num.Substring(0, 3);
            String g2 = num.Substring(3, 3);
            String g3 = num.Substring(6, 3);
            String g4 = num.Substring(9, 3);
            String g5 = num.Substring(12, 3);
            String g6 = num.Substring(15, 3);

            //read group1 ---------------------
            if (!g1.Equals("000"))
            {
                temp = readGroup(g1);
                temp += " Triệu";
            }
            //read group2-----------------------
            if (!g2.Equals("000"))
            {
                temp += readGroup(g2);
                temp += " Nghìn";
            }
            //read group3 ---------------------
            if (!g3.Equals("000"))
            {
                temp += readGroup(g3);
                temp += " Tỷ";
            }
            else if (!"".Equals(temp))
            {
                temp += " Tỷ";
            }

            //read group2-----------------------
            if (!g4.Equals("000"))
            {
                temp += readGroup(g4);
                temp += " Triệu";
            }
            //---------------------------------
            if (!g5.Equals("000"))
            {
                temp += readGroup(g5);
                temp += " Nghìn";
            }
            //-----------------------------------

            temp = temp + readGroup(g6);
            //---------------------------------
            // Refine
            temp = temp.Replace("Một Mươi", "Mười");
            temp = temp.Trim();
            temp = temp.Replace("Không Trăm", "");
            //        if (temp.indexOf("Không Trăm") == 0) temp = temp.substring(10);
            temp = temp.Trim();
            temp = temp.Replace("Mười Không", "Mười");
            temp = temp.Trim();
            temp = temp.Replace("Mươi Không", "Mươi");
            temp = temp.Trim();
            if (temp.IndexOf("Lẻ") == 0) temp = temp.Substring(2);
            temp = temp.Trim();
            temp = temp.Replace("Mươi Một", "Mươi Mốt");
            temp = temp.Trim();

            //Change Case
            return (temp.Substring(0, 1).ToLower() + temp.Substring(1).ToLower() + " đồng chẵn").ToUpperInvariant();
        }
    }
}
