using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using VPK_ERP_API.Models;

namespace VPK_ERP_API.Utilities
{

    public class CauTrucMenu
    {
        public string FunctionName { get; set; }
        public int RowID { get; set; }
        public int Parent { get; set; }

        public string URL { get; set; }

        public string Icon { get; set; }

        public List<CauTrucMenu> Children { get; set; }
    }

    public class Toolkits
    {

        VPK_ERPEntities ett = new VPK_ERPEntities();



        public List<CauTrucMenu> Select_All_Menu2_Huu_2(int EmployeeID = 4)
        {



            var query = ett.Database.SqlQuery<CauTrucMenu>(@"SELECT a.FunctionName,
                                           a.RowID,
                                           a.Parent, a.URL, a.Icon
                                    FROM dbo.[Function] AS a
                                        INNER JOIN dbo.Employee_Category_Function AS b
                                            ON a.RowID = b.RowIDFunction
                                        INNER JOIN dbo.Employee_Category AS c
                                            ON b.RowIDEmployeeCategory = c.RowID
                                        INNER JOIN dbo.Employee_Employee_Category AS d
                                            ON c.RowID = d.RowIDEmployeeCategory
                                    WHERE a.IsDelete = 0
                                          AND d.RowIDEmployee = " + EmployeeID + " ORDER BY a.[Order] ASC").ToList();

            if (query.Count > 0)
            {
                return XayDungCayMenu(query, 0);
            }
            else
            {
                return null;
            }







        }

        public List<CauTrucMenu> XayDungCayMenu(List<CauTrucMenu> arayMenu, int parentId = 0)
        {

            List<CauTrucMenu> MultiMenu = new List<CauTrucMenu>();

            foreach (var element in arayMenu.ToList())
            {

                if (element.Parent == parentId)
                {
                    var children = XayDungCayMenu(arayMenu, element.RowID);
                    if (children.Count > 0)
                    {
                        element.Children = children;
                    }

                    MultiMenu.Add(element);
                    arayMenu.Remove(element);

                }

                //MultiMenu = MultiMenu.ToList();

            }

            return MultiMenu;



        }




        public double TinhToanGiaCongTrinh(double giaTienThietKeKienTruc, double giaTienThietKeNoiThat, double dienTich, bool IsThietKeKienTruc, bool IsThietKeNoiThat, bool IsThietKeTronGoi, bool IsTanCoDien)
        {
            double tongTienThietKeKienTruc = 0;
            double tongTienThietKeNoiThat = 0;

            double tongTienCuoiCung = 0;


            #region "Thiết kế kiến trúc"
            //tính ra diện tích
            if (IsThietKeKienTruc == true)
            {
                if (dienTich >= 150 && dienTich <= 250)
                {


                    tongTienThietKeKienTruc = giaTienThietKeKienTruc * dienTich * 1;


                }
                else
                {
                    if (dienTich >= 250 && dienTich <= 450)
                    {
                        tongTienThietKeKienTruc = giaTienThietKeKienTruc * dienTich * 1.1;

                    }
                    else
                    {

                        if (dienTich >= 450 && dienTich <= 900)
                        {
                            tongTienThietKeKienTruc = giaTienThietKeKienTruc * dienTich * 0.9;

                        }
                        else
                        {

                            tongTienThietKeKienTruc = -1;

                        }
                    }

                }



                if (IsTanCoDien == true)
                {

                    tongTienThietKeKienTruc = tongTienThietKeKienTruc * 1.2;
                }

            }

            #endregion



            #region "Thiết kế nội thất"
            //tính ra diện tích

            if (IsThietKeNoiThat == true)
            {
                if (dienTich >= 150 && dienTich <= 250)
                {


                    tongTienThietKeNoiThat = giaTienThietKeNoiThat * dienTich * 1;


                }
                else
                {

                    if (dienTich >= 250 && dienTich <= 450)
                    {


                        tongTienThietKeNoiThat = giaTienThietKeNoiThat * dienTich * 1.1;


                    }
                    else
                    {

                        if (dienTich >= 450 && dienTich <= 900)
                        {
                            tongTienThietKeNoiThat = giaTienThietKeNoiThat * dienTich * 0.9;

                        }
                        else
                        {

                            tongTienThietKeNoiThat = -1;

                        }

                    }
                }



                if (IsTanCoDien == true)
                {

                    tongTienThietKeNoiThat = tongTienThietKeNoiThat * 1.2;
                }

            }

            #endregion




            tongTienCuoiCung = tongTienThietKeKienTruc + tongTienThietKeNoiThat;




            return Math.Round(tongTienCuoiCung, 0);





        }




    }




}