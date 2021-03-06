using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using CommonClasses;
using SharpNeat.Utility;
using System.Collections.Generic;

namespace TPC.C
{
    public class Tpcc
    {
        private static readonly FastRandom Random = new FastRandom();
        public int Clientid;
        // ReSharper disable UnusedMember.Global
        public int Maxrange;
        public int Minrange;
        public int Runtime;
        // ReSharper restore UnusedMember.Global
        private static void NewOrder(int inseq)
        {
            /***********************
             * runs roughly 25 new orders a second at 2.4ghz core 2 duo
             *
             */
            string query;
            using (var objConnect = new SqlConnection(Globals.StrPublisherConn))
            {
                SqlCommand _objCommand;

                try
                {
                    objConnect.Open();
                    int autoNumber = inseq;
                    if (Globals.StoredProc)
                    {
                        int vw = Globals.RandomWH();
                        query = "exec CREATE_NEW_ORDER NULL,1,'OL_NUM_1'," +vw + "," + autoNumber + ";";
                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600, CommandType = CommandType.Text };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                    else
                    {
                        const float voAllLocal = 1;
                        const string volNumber = "OL_NUM";

                        float vsOrderCnt = 1;
                        /* Generating Random Values */
                        const float volAmount = 0;
                        int maxValue = 500001;
                        int minValue = 100001;
                        float vstoId = Random.Next(minValue, maxValue);
                        int vdisId = Random.Next(minValue, maxValue);
                        int vnewordId = Random.Next(minValue, maxValue);
                        int voordId = Random.Next(minValue, maxValue);
                        int vordlineId = Random.Next(minValue, maxValue);
                        int vrwId = Globals.RandomWH();
                        string vwId = "W_" + Convert.ToString(vrwId);
                        maxValue = 11;
                        minValue = 1;
                        float vrdId = Random.Next(minValue, maxValue);
                        string vdId = "D_W" + Convert.ToString(vrwId) + "_";
                        vdId = vdId + Convert.ToString(vrdId);
                        maxValue = 100001;
                        minValue = 1;
                        float viId = Random.Next(minValue, maxValue);
                        maxValue = 5;
                        minValue = 1;
                        int volQuantity = Random.Next(minValue, maxValue);
                        maxValue = 3000;
                        minValue = 1;
                        int randomInteger = Random.Next(minValue, maxValue);
                        string vcId = "C_W" + vrwId + "_D" + vrdId + "_" + Convert.ToString(randomInteger);
                        float vdNextOId = 1;
                        const float voOlCnt = 1;
                        int vsQuantity = 1;
                        SqlDataReader result = null;
                        /* Selecting the Sequence Value */

                        query = "SELECT  \r\n " +
                                "S_DIST_01, \r\n " +
                                "S_DIST_02, \r\n " +
                                "S_DIST_03, \r\n " +
                                "S_DIST_04, \r\n " +
                                "S_DIST_05, \r\n " +
                                "S_DIST_06, \r\n " +
                                "S_DIST_07, \r\n " +
                                "S_DIST_08, \r\n " +
                                "S_DIST_09, \r\n " +
                                "S_DIST_10, \r\n " +
                                "S_QUANTITY, \r\n " +
                                "S_DATA, \r\n " +
                                "S_YTD, \r\n " +
                                "S_ORDER_CNT \r\n " +
                                "FROM  stock \r\n " +
                                "WHERE	 S_I_ID  = " + viId + " \r\n " +
                                " AND	S_W_ID  = '" + vwId + "' \r\n";

                        try
                        {
                            using (_objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = _objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    result["S_DIST_01"].ToString();
                                    result["S_DIST_02"].ToString();
                                    result["S_DIST_03"].ToString();
                                    result["S_DIST_04"].ToString();
                                    result["S_DIST_05"].ToString();
                                    result["S_DIST_06"].ToString();
                                    result["S_DIST_07"].ToString();
                                    result["S_DIST_08"].ToString();
                                    result["S_DIST_09"].ToString();
                                    result["S_DIST_10"].ToString();
                                    result["S_DATA"].ToString();
                                    Convert.ToSingle(result["S_YTD"].ToString());

                                    vsQuantity = Convert.ToInt32(result["S_QUANTITY"].ToString());
                                    vsOrderCnt = Convert.ToSingle(result["S_ORDER_CNT"].ToString());
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        if (vsQuantity < volQuantity)
                        {
                            vsQuantity = vsQuantity - volQuantity + 91;
                        }
                        else
                        {
                            vsQuantity = vsQuantity - volQuantity;
                        }

                        //    /* Updating the Sotck Table */
                        query = "UPDATE  STOCK \r\n" +
                                "SET	S_QUANTITY = " + Convert.ToString(vsQuantity) + " , \r\n" +
                                "S_ORDER_CNT = " + Convert.ToString(vsOrderCnt + 1) + ", \r\n" +
                                "SEQ_ID = " + vstoId + " \r\n" +
                                "WHERE  S_I_ID  = " + viId + " \r\n" +
                                "AND	S_W_ID  = '" + vwId + "' \r\n";

                        //Console.WriteLine(_query);
                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    -- IMPLICIT_TRANSACTIONS is set to OFF
                        //    /* Selecting the Custome Table Details */
                        query = "SELECT  \r\n" +
                                "C_LAST,  \r\n" +
                                "C_CREDIT,  \r\n" +
                                "C_DISCOUNT \r\n" +
                                "FROM  CUSTOMER  \r\n" +
                                "WHERE	 C_W_ID  = '" + vwId + "' \r\n" +
                                "AND	C_D_ID  = '" + vdId + "' \r\n" +
                                "AND	C_ID  = '" + vcId + "' \r\n";

                        //Console.WriteLine(_query);
                        try
                        {
                            using (_objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = _objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    result["C_LAST"].ToString();
                                    result["C_CREDIT"].ToString();
                                    Convert.ToSingle(result["C_DISCOUNT"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    /* Selecting the District Table Details */
                        query = "SELECT  \r\n" +
                                "   D_TAX, \r\n" +
                                "   D_NEXT_O_ID \r\n" +
                                "FROM  DISTRICT  \r\n" +
                                "WHERE	 D_W_ID  = '" + vwId + "' \r\n" +
                                "AND	D_ID  = '" + vdId + "' \r\n";
                        try
                        {
                            using (_objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = _objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    Convert.ToSingle(result["D_TAX"].ToString());
                                    vdNextOId = Convert.ToSingle(result["D_NEXT_O_ID"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    /* Updating the District Table */
                        query = "UPDATE  DISTRICT    \r\n" +
                                "SET	D_NEXT_O_ID = " + Convert.ToString((vdNextOId + 1)) + ",	 \r\n" +
                                "   SEQ_ID = " + Convert.ToString(vdisId) + "   \r\n" +
                                "WHERE  D_W_ID  = '" + vwId + "' \r\n" +
                                "AND	D_ID  = '" + vdId + "' \r\n";

                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    /* Inserting into Order Table */
                        query = "INSERT INTO  O_ORDER    \r\n" +
                                "(   \r\n" +
                                "O_ID , \r\n" +
                                "O_D_ID ,  \r\n" +
                                "O_W_ID ,  \r\n" +
                                "O_C_ID ,  \r\n" +
                                "O_ENTRY_D ,  \r\n" +
                                "O_OL_CNT ,  \r\n" +
                                "O_ALL_LOCAL ,  \r\n" +
                                "SEQ_ID )   \r\n" +
                                "VALUES 		(    \r\n" +
                                " " + autoNumber + ", \r\n" +
                                " '" + vdId + "', \r\n" +
                                " '" + vwId + "', \r\n" +
                                " '" + vcId + "', \r\n" +
                                "getdate() , \r\n" +
                                " " + voOlCnt + ", \r\n" +
                                " " + voAllLocal + ", \r\n" +
                                " " + voordId + ") \r\n";
                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                        //    /* Inserting into New Order Table */
                        query = "INSERT INTO  NEW_ORDER   \r\n" +
                                "(   \r\n" +
                                "NO_O_ID, \r\n" +
                                "NO_D_ID, \r\n" +
                                "NO_W_ID,  \r\n" +
                                "SEQ_ID ) \r\n" +
                                "VALUES 		(  \r\n" +
                                " " + autoNumber + " , \r\n" +
                                " '" + vdId + "' , \r\n" +
                                " '" + vwId + "' , \r\n" +
                                " " + vnewordId + " ) \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    -- IMPLICIT_TRANSACTIONS is set to OFF
                        //    /* Selecting the details of Warehouse Table */
                        query = "SELECT W_TAX \r\n" +
                                "FROM  WAREHOUSE  \r\n" +
                                "WHERE	 W_ID  = '" + vwId + "' \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (_objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = _objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    Convert.ToSingle(result["W_TAX"]);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    /* Selecting the details of Item Table */
                        query = "SELECT I_PRICE \r\n" +
                                "FROM  ITEM  \r\n" +
                                "WHERE	 I_ID  = '" + viId + "' \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (_objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = _objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    Convert.ToSingle(result["I_PRICE"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        //    /* Inserting into Orde Line Table */
                        query = "    INSERT INTO ORDER_LINE    \r\n" +
                                "( \r\n" +
                                "OL_O_ID , \r\n" +
                                "OL_D_ID ,  \r\n" +
                                "OL_W_ID ,  \r\n" +
                                "OL_NUMBER ,  \r\n" +
                                "OL_I_ID ,  \r\n" +
                                "OL_SUPPLY_W_ID ,  \r\n" +
                                "OL_QUANTITY ,  \r\n" +
                                "OL_AMOUNT ,  \r\n" +
                                "SEQ_ID )   \r\n" +
                                "VALUES 		(  \r\n" +
                                " " + autoNumber + " , \r\n" +
                                " '" + vdId + "' , \r\n" +
                                " '" + vwId + "' , \r\n" +
                                " '" + volNumber + "' , \r\n" +
                                " '" + viId + "' , \r\n" +
                                " '" + vwId + "' , \r\n" +
                                " " + volQuantity + " , \r\n" +
                                " " + volAmount + " , \r\n" +
                                " " + vordlineId + " ) \r\n";
                        try
                        {
                            _objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            _objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                    }

                }
                catch (Exception e)
                {

                    Errhandle.StopProcessing(e, "");
                }
            }
        }

        private static void OrderStatus()
        {
            string query;
            SqlCommand objCommand;
            SqlDataReader result = null;
            int rw = Globals.RandomWH();
            int rd = Random.Next(11);
            int rc = Random.Next(3001);
            //int ram = Random.Next(1000);
            //$callstmt = "exec ORDER_STATUS '$W_ID', '$D_ID', '$C_ID';";

            string voId = null;

            if (rd == 0)
            {
                rd = 1;
            }
            if (rc == 0)
            {
                rc = 1;
            }

            string vwId = "W_" + Convert.ToString(rw);
            string vdId = "D_W" + Convert.ToString(rw) + "_" + Convert.ToString(rd);
            string vcId = "C_W" + Convert.ToString(rw) + "_D" + Convert.ToString(rd) + "_" + Convert.ToString(rc);

            using (var objConnect = new SqlConnection(Globals.StrPublisherConn))
            {
                try
                {
                    objConnect.Open();

                    if (Globals.StoredProc)
                    {
                        query = "exec ORDER_STATUS '" + vwId + "', '" + vdId + "', '" + vcId + "';";
                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                    else
                    {
                        query = "SELECT \r\n" +
                                "isnull(C_BALANCE,0) C_BALANCE, \r\n" +
                                "C_FIRST, \r\n" +
                                "C_MIDDLE, \r\n" +
                                "C_LAST \r\n" +
                                "FROM  CUSTOMER  \r\n" +
                                "WHERE	 C_W_ID  = '" + vwId + "' \r\n" +
                                "AND	C_D_ID  = '" + vdId + "' \r\n" +
                                "AND	C_ID  = '" + vcId + "' \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    Convert.ToSingle(result["C_BALANCE"].ToString());
                                    result["C_FIRST"].ToString();
                                    result["C_MIDDLE"].ToString();
                                    result["C_LAST"].ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        query = "SELECT \r\n" +
                                "O_ID, \r\n" +
                                "O_CARRIER_ID, \r\n" +
                                "O_ENTRY_D \r\n" +
                                "FROM  O_ORDER  \r\n" +
                                "WHERE	 O_W_ID  = '" + vwId + "' \r\n" +
                                "AND	O_D_ID  = '" + vdId + "' \r\n" +
                                "AND	O_C_ID  = '" + vcId + "' \r\n" +
                                "ORDER BY O_W_ID DESC, \r\n" +
                                "O_D_ID DESC, \r\n" +
                                "O_C_ID DESC, \r\n" +
                                "O_ID DESC  \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    voId = result["O_ID"].ToString();
                                    result["O_CARRIER_ID"].ToString();
                                    Convert.ToDateTime(result["O_ENTRY_D"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        query = "SELECT  \r\n" +
                                "OL_I_ID,  \r\n" +
                                "OL_SUPPLY_W_ID,  \r\n" +
                                "OL_QUANTITY,  \r\n" +
                                "OL_AMOUNT,  \r\n" +
                                "isnull(OL_DELIVERY_D,'01-01-1972 00:00:00.000') OL_DELIVERY_D  \r\n" +
                                "FROM  ORDER_LINE   \r\n" +
                                "WHERE	 OL_W_ID  = '" + vwId + "' \r\n" +
                                "AND	OL_D_ID  = '" + vdId + "' \r\n" +
                                "AND	OL_O_ID  = '" + voId + "' \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    result["OL_I_ID"].ToString();
                                    result["OL_SUPPLY_W_ID"].ToString();
                                    Convert.ToSingle(result["OL_QUANTITY"].ToString());
                                    Convert.ToSingle(result["OL_AMOUNT"].ToString());
                                    Convert.ToDateTime(result["OL_DELIVERY_D"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                }
                catch (Exception e)
                {

                    Errhandle.StopProcessing(e, "");
                }
            }
        }

        private static void Payment()
        {
            string query;
            SqlCommand objCommand;
            SqlDataReader result = null;

            int rw = Globals.RandomWH();
            int rd = Random.Next(11);
            int rc = Random.Next(3001);
            int ram = Random.Next(1000);


            //string VC1_LAST;

            //		    float NAMECNT;
            //		    float N;
            //		    string VCC_ID;
            //		    string VC_STDDLE;

            float vcBalance = 1;
            string vcCredit = null;
            string vcData = null;
            float vcPaymentCnt = 1;
            const string vcusId = null;
            float vdYtd = 1;
            const string vdisId = null;
            float vwYtd = 1;
            const string vwarId = null;
            string vwName = null;
            string vdName = null;
            const string vhisId = null;
            
            if (rd == 0)
            {
                rd = 1;
            }
            if (rc == 0)
            {
                rc = 1;
            }

            string wId = "W_" + Convert.ToString(rw);
            string dId = "D_W" + Convert.ToString(rw) + "_" + Convert.ToString(rd);
            string cId = "C_W" + Convert.ToString(rw) + "_D" + Convert.ToString(rd) + "_" + Convert.ToString(rc);

            string vwId = wId;
            string vdId = dId;
            string vcId = cId;
            float vhAmount = ram;

            using (var objConnect = new SqlConnection(Globals.StrPublisherConn))
            {
                try
                {
                    objConnect.Open();

                    if (Globals.StoredProc)
                    {
                        query = "exec PAYMENT '" + vwId + "', '" + vdId + "', '" + vcId + "', 'AVS', " + ram + ";";
                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                    else
                    {
                        query = "SELECT  \r\n" +
                                "C_FIRST,  \r\n" +
                                "C_MIDDLE,  \r\n" +
                                "C_LAST,  \r\n" +
                                "C_STREET_1,  \r\n" +
                                "C_STREET_2,  \r\n" +
                                "C_CITY,  \r\n" +
                                "C_STATE,  \r\n" +
                                "C_ZIP,  \r\n" +
                                "C_PHONE,  \r\n" +
                                "C_SINCE,  \r\n" +
                                "C_CREDIT,  \r\n" +
                                "C_CREDIT_LIM,  \r\n" +
                                "C_DISCOUNT,  \r\n" +
                                "isnull(C_BALANCE,0) C_BALANCE,  \r\n" +
                                "C_PAYMENT_CNT,  \r\n" +
                                "isnull(C_DATA,'') C_DATA  \r\n" +
                                "FROM  CUSTOMER   \r\n" +
                                "WHERE	 C_W_ID  =  '" + vwId + "' \r\n" +
                                "AND	C_D_ID  = '" + vdId + "' \r\n" +
                                "AND	C_ID  = '" + vcId + "' \r\n";
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    var vcFirst = result["C_FIRST"].ToString();
                                    var vcMiddle = result["C_MIDDLE"].ToString();
                                    var vcLast = result["C_LAST"].ToString();
                                    var vcStreet1 = result["C_STREET_1"].ToString();
                                    var vcStreet2 = result["C_STREET_2"].ToString();
                                    var vcCity = result["C_CITY"].ToString();
                                    var vcState = result["C_STATE"].ToString();
                                    var vcZip = result["C_ZIP"].ToString();
                                    var vcPhone = result["C_PHONE"].ToString();
                                    var vcSince = Convert.ToDateTime(result["C_SINCE"].ToString());
                                    vcCredit = result["C_CREDIT"].ToString();
                                    var vcCreditLim = Convert.ToSingle(result["C_CREDIT_LIM"].ToString());
                                    var vcDiscount = Convert.ToSingle(result["C_DISCOUNT"].ToString());
                                    vcBalance = Convert.ToSingle(result["C_BALANCE"].ToString());
                                    vcPaymentCnt = Convert.ToSingle(result["C_PAYMENT_CNT"].ToString());
                                    vcData = result["C_DATA"].ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        vcBalance = vcBalance + vhAmount;
                        if (vcCredit == "BC")
                        {
                            query = "UPDATE  CUSTOMER \r\n" +
                                    "SET	C_BALANCE = " + vcBalance + ", \r\n" +
                                    "C_DATA = " + Convert.ToString(vcId) + Convert.ToString(vdId) + Convert.ToString(vwId) +
                                    Convert.ToString(vdId) + Convert.ToString(vwId) + Convert.ToString(vhAmount) +
                                    Convert.ToString(vcData) + ", \r\n" +
                                    "C_PAYMENT_CNT = " + (vcPaymentCnt + 1) + ",	 \r\n" +
                                    "SEQ_ID = " + (vcusId + 1) + " \r\n" +
                                    "WHERE  C_W_ID  = " + vwId + " \r\n" +
                                    "AND	C_D_ID  = " + vdId + " \r\n" +
                                    "AND	C_ID  = " + vcId + " \r\n";
                            try
                            {
                                objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                                objCommand.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Errhandle.StopProcessing(e, query);
                            }
                            //_query is the same as above without the c_data column set...
                            query = "UPDATE  CUSTOMER \r\n" +
                                    "SET	C_BALANCE = " + vcBalance + ", \r\n" +
                                    "C_PAYMENT_CNT = " + (vcPaymentCnt + 1) + ",	 \r\n" +
                                    "SEQ_ID = " + (vcusId + 1) + " \r\n" +
                                    "WHERE  C_W_ID  = " + vwId + " \r\n" +
                                    "AND	C_D_ID  = " + vdId + " \r\n" +
                                    "AND	C_ID  = " + vcId + " \r\n";
                            try
                            {
                                objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                                objCommand.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Errhandle.StopProcessing(e, query);
                            }
                        }

                        query = "SELECT \r\n" +
                                "D_STREET_1, \r\n" +
                                "D_STREET_2, \r\n" +
                                "D_CITY, \r\n" +
                                "D_STATE, \r\n" +
                                "D_ZIP, \r\n" +
                                "D_NAME, \r\n" +
                                "D_YTD \r\n" +
                                "FROM  DISTRICT  \r\n" +
                                "WHERE	 D_W_ID  = '" + vwId + "' \r\n" +
                                "AND	D_ID  =  '" + vdId + "' \r\n";
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    var vdStreet1 = result["D_STREET_1"].ToString();
                                    var vdStreet2 = result["D_STREET_2"].ToString();
                                    var vdCity = result["D_CITY"].ToString();
                                    var vdState = result["D_STATE"].ToString();
                                    var vdZip = result["D_ZIP"].ToString();
                                    vdName = result["D_NAME"].ToString();
                                    vdYtd = Convert.ToSingle(result["D_YTD"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        query = "UPDATE  DISTRICT \r\n" +
                                "SET	D_YTD = " + (vdYtd + vhAmount) + ",	\r\n" +
                                "SEQ_ID = " + (vdisId + 1) + " \r\n" +
                                "WHERE  D_W_ID  = '" + vwId + "' \r\n" +
                                "AND	D_ID  = '" + vdId + "' \r\n";

                        //Console.WriteLine(_query);                               
                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        query = "SELECT \r\n" +
                                "W_NAME, \r\n" +
                                "W_STREET_1, \r\n" +
                                "W_STREET_2, \r\n" +
                                "W_CITY, \r\n" +
                                "W_STATE, \r\n" +
                                "W_ZIP, \r\n" +
                                "W_YTD \r\n" +
                                "FROM  WAREHOUSE  \r\n" +
                                "WHERE	 W_ID  = '" + vwId + "' \r\n";

                        //Console.WriteLine(_query);                               
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {

                                while (result.Read())
                                {
                                    vwName = result["W_NAME"].ToString();
                                    //_result["W_STATE"].ToString();
                                    //_result["W_ZIP"].ToString();
                                    vwYtd = Convert.ToSingle(result["W_YTD"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                        query = "UPDATE  WAREHOUSE \r\n" +
                                "SET W_YTD = " + (vwYtd + vhAmount) + ", \r\n" +
                                "SEQ_ID = " + (vwarId + 1) + " \r\n" +
                                "WHERE  W_ID  = '" + vwId + "' \r\n";

                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        string vhData = vwName + "" + vdName;
                        query = "INSERT INTO  HISTORY    \r\n" +
                                "( H_C_D_ID ,  \r\n" +
                                "H_C_W_ID ,  \r\n" +
                                "H_C_ID ,  \r\n" +
                                "H_D_ID ,  \r\n" +
                                "H_W_ID ,  \r\n" +
                                "H_DATE ,  \r\n" +
                                "H_AMOUNT ,  \r\n" +
                                "H_DATA ,  \r\n" +
                                "SEQ_ID )   \r\n" +
                                "VALUES 		(  \r\n" +
                                " '" + vdId + "', \r\n" +
                                " '" + vwId + "', \r\n" +
                                " '" + vcId + "', \r\n" +
                                " '" + vdId + "', \r\n" +
                                " '" + vwId + "', \r\n" +
                                " getdate(), \r\n" +
                                " " + vhAmount + ", \r\n" +
                                " '" + vhData + "', \r\n" +
                                " " + (vhisId + 1) + " \r\n" +
                                ") \r\n";

                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                }
                catch (Exception e)
                {

                    Errhandle.StopProcessing(e, "");
                }
            }
        }

        private static void Delivery()
        {
            string query;
            int rw = Globals.RandomWH();
            int rd = Random.Next(11);
            
            if (rd == 0)
            {
                rd = 1;
            }

            string vwId = "W_" + Convert.ToString(rw);
            string vdId = "D_W" + Convert.ToString(rw) + "_" + Convert.ToString(rd);
            float volAmount = 1;
            string voCarrierId = null;
            string voCId = null;

            using (var objConnect = new SqlConnection(Globals.StrPublisherConn))
            {
                try
                {
                    objConnect.Open();

                    if (Globals.StoredProc)
                    {
                        query = "exec DELIVERY '" + vwId + "', '" + vdId + "';";
                        try
                        {
                            using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                    else
                    {
                        query = "SELECT NO_O_ID \r\n" +
                                "FROM  NEW_ORDER  \r\n" +
                                "WHERE	 NO_W_ID  = '" + vwId + "' \r\n" +
                                "AND	NO_D_ID  = '" + vdId + "' \r\n" +
                                "ORDER BY NO_O_ID DESC  \r\n";
                        //Console.WriteLine(_query);                        
                        try
                        {
                            List<string> orders = new List<string>();

                            using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (var result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    orders.Add(result["NO_O_ID"].ToString());
                                }
                            }
                            foreach (var vnoOId in orders)
                            {
                                //float OL_TOTAL;

                                const float vordlineId = 100001;
                                const float voordId = 100001;
                                const int vcusId = 1;

                                query = "DELETE FROM   NEW_ORDER \r\n" +
                                        "WHERE  NO_O_ID  = '" + vnoOId + "' \r\n" +
                                        "AND	NO_D_ID  = '" + vdId + "' \r\n" +
                                        "AND	NO_W_ID  = '" + vwId + "' \r\n";
                                try
                                {
                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                        objCommand.ExecuteNonQuery();
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }

                                query = "SELECT \r\n" +
                                        "O_C_ID, \r\n" +
                                        "isnull(O_CARRIER_ID,' ') O_CARRIER_ID \r\n" +
                                        "FROM  O_ORDER  \r\n" +
                                        "WHERE	 O_ID  = " + vnoOId + " \r\n" +
                                        "AND	O_D_ID  = '" + vdId + "' \r\n" +
                                        "AND	O_W_ID  = '" + vwId + "' \r\n";
                                //Console.WriteLine(_query);
                                try
                                {
                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                    using (var result2 = objCommand.ExecuteReader())
                                    {
                                        while (result2.Read())
                                        {
                                            voCId = result2["O_C_ID"].ToString();
                                            voCarrierId = result2["O_CARRIER_ID"].ToString();
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }

                                query = "UPDATE  O_ORDER \r\n" +
                                        "SET	O_CARRIER_ID = '" + voCarrierId + "', \r\n" +
                                        "SEQ_ID = " + Convert.ToString((voordId + 1)) + "  \r\n" +
                                        "WHERE  O_ID  = '" + vnoOId + "'  \r\n" +
                                        "AND	O_D_ID  = '" + vdId + "'  \r\n" +
                                        "AND	O_W_ID  = '" + vwId + "'  \r\n";
                                try
                                {
                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                        objCommand.ExecuteNonQuery();
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }

                                query = "SELECT SUM(CONVERT(FLOAT, OL_AMOUNT)) OL_AMOUNT \r\n" +
                                        "FROM  ORDER_LINE  \r\n" +
                                        "WHERE	 OL_O_ID  = " + vnoOId + " \r\n" +
                                        "AND	OL_D_ID  = '" + vdId + "' \r\n" +
                                        "AND	OL_W_ID  = '" + vwId + "' \r\n";
                                //Console.WriteLine(_query);
                                try
                                {
                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                    using (var result3 = objCommand.ExecuteReader())
                                    {
                                        while (result3.Read())
                                        {
                                            volAmount = Convert.ToSingle(result3["OL_AMOUNT"].ToString());

                                            query = "UPDATE  ORDER_LINE \r\n" +
                                                    "SET	OL_DELIVERY_D = getdate(), \r\n" +
                                                    "SEQ_ID = " + (vordlineId + 1) + " \r\n" +
                                                    "WHERE  OL_O_ID  = " + vnoOId + " \r\n" +
                                                    "AND	OL_D_ID  = '" + vdId + "' \r\n" +
                                                    "AND	OL_W_ID  = '" + vwId + "' \r\n";
                                            //Console.WriteLine(_query);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }

                                try
                                {
                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                        objCommand.ExecuteNonQuery();
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }

                                query = "SELECT \r\n" +
                                        "isnull(C_BALANCE,0) C_BALANCE, \r\n" +
                                        "C_DELIVERY_CNT \r\n" +
                                        "FROM  CUSTOMER  \r\n" +
                                        "WHERE	 C_W_ID  = '" + vwId + "' \r\n" +
                                        "AND	C_D_ID  = '" + vdId + "' \r\n" +
                                        "AND	C_ID  = '" + voCId + "' \r\n";
                                //Console.WriteLine(_query);
                                try
                                {
                                    List<float[]> values = new List<float[]>();

                                    using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                    using (var result4 = objCommand.ExecuteReader())
                                    {
                                        while (result4.Read())
                                        {
                                            var v = new float[] {
                                                Convert.ToSingle(result4["C_BALANCE"].ToString()),
                                                Convert.ToSingle(result4["C_DELIVERY_CNT"].ToString()) };
                                            values.Add(v);
                                        }
                                    }
                                    foreach (float[] v in values)
                                    {
                                        float vcBalance = v[0];
                                        float vcDeliveryCnt = v[1];
                                            query = "UPDATE  CUSTOMER \r\n" +
                                                    "SET	C_BALANCE = " + Convert.ToString((vcBalance + volAmount)) + ", \r\n" +
                                                    "C_DELIVERY_CNT = " + Convert.ToString((vcDeliveryCnt + 1)) + ", \r\n" +
                                                    "SEQ_ID = " + Convert.ToString((vcusId + 1)) + " \r\n" +
                                                    "WHERE  C_ID  = '" + voCId + "' \r\n" +
                                                    "AND	C_D_ID  = '" + vdId + "' \r\n" +
                                                    "AND	C_W_ID  =  '" + vwId + "' \r\n";
                                            try
                                            {
                                                using (var objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                                                    objCommand.ExecuteNonQuery();
                                            }
                                            catch (Exception e)
                                            {
                                                Errhandle.StopProcessing(e, query);
                                            }
                                     }
                                }
                                catch (Exception e)
                                {
                                    Errhandle.StopProcessing(e, query);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                }
                catch (Exception e)
                {

                    Errhandle.StopProcessing(e, "");
                }
            }
        }

        private static void StockLevel()
        {
            string query;
            
            SqlCommand objCommand;
            SqlDataReader result = null;
            int rw = Globals.RandomWH();
            int rd = Random.Next(11);
            int vdNextOId = 0;
            
            if (rd == 0)
            {
                rd = 1;
            }

            string vwId = "W_" + Convert.ToString(rw);
            string vdId = "D_W" + Convert.ToString(rw) + "_" + Convert.ToString(rd);
            using (var objConnect = new SqlConnection(Globals.StrPublisherConn))
            {
                try
                {
                    objConnect.Open();


                    if (Globals.StoredProc)
                    {
                        query = "exec STOCK_LEVEL '" + vwId + "', '" + vdId + "';";
                        try
                        {
                            objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                            objCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                    else
                    {
                        query = "SELECT D_NEXT_O_ID \r\n" +
                                "FROM  DISTRICT  \r\n" +
                                "WHERE	 D_W_ID  = '" + vwId + "' \r\n" +
                                "AND	D_ID  = '" + vdId + "' \r\n";
                        //Console.WriteLine(_query);                            
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    vdNextOId = Convert.ToInt32(result["D_NEXT_O_ID"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }

                        query = "SELECT COUNT(DISTINCT S_I_ID) N_ITEMS \r\n" +
                                "FROM  ORDER_LINE, \r\n" +
                                "STOCK  \r\n" +
                                "WHERE	 OL_W_ID  = '" + vwId + "' \r\n" +
                                "AND	OL_D_ID  = '" + vdId + "' \r\n" +
                                "AND	OL_O_ID  <  " + vdNextOId + " \r\n" +
                                "AND	OL_O_ID  >=  " + (vdNextOId - 20) + " \r\n" +
                                "AND	S_W_ID  =  '" + vwId + "' \r\n" +
                                "AND	S_I_ID  =  OL_I_ID \r\n";
                        //Console.WriteLine(_query);
                        try
                        {
                            using (objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 })
                            using (result = objCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    var nItems = Convert.ToSingle(result["N_ITEMS"].ToString());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Errhandle.StopProcessing(e, query);
                        }
                    }
                }
                catch (Exception e)
                {

                    Errhandle.StopProcessing(e, "");
                }
            }
        }

        public object Client(Object stateinfo)
        {
#pragma warning disable 219
            int i;
            int cnt = 0;
            int no = 0;
            int os = 0;
            int p = 0;
            int d = 0;
            int sl = 0;
#pragma warning restore 219

            int min = Clientid * 1000000;

            DateTime currdate = DateTime.Now;
            DateTime targetdate = currdate + new TimeSpan(0, Globals.MaxRunTimeMin, 0);

            int sleep = Globals.ClientSleepSec;

            Globals.NumLoops = (Globals.NumLoops == 0) ? int.MaxValue : Globals.NumLoops;

            while ((cnt < Globals.NumLoops) && (currdate < targetdate))
            {
                if (Globals.RawWrite == 1)
                {
                    RawWrites();
                }
                else
                {
                    i = Random.Next(1, 100);
                    //Console.WriteLine(i);
                    if (i <= Globals.PNO)
                    {
                        NewOrder(min); //45
                        no++;
                    }
                    else if ((i > Globals.PNO) && (i <= (Globals.PNO + Globals.POS)))
                    {
                        OrderStatus(); //43
                        os++;
                    }
                    else if ((i > (Globals.PNO + Globals.POS)) && (i <= (Globals.PNO + Globals.POS + Globals.PP)))
                    {
                        Payment(); //4
                        p++;
                    }
                    else if ((i > (Globals.PNO + Globals.POS + Globals.PP)) &&
                             (i <= (Globals.PNO + Globals.POS + Globals.PP + Globals.PD)))
                    {
                        Delivery(); //4
                        d++;
                    }
                    else if ((i > (Globals.PNO + Globals.POS + Globals.PP + Globals.PD)) &&
                             (i <= (Globals.PNO + Globals.POS + Globals.PP + Globals.PD + Globals.PSL)))
                    {
                        StockLevel(); //4
                        sl++;
                    }
                    min++;
                }
                if (sleep != 0)
                {
                    Thread.Sleep(sleep);
                }
                currdate = DateTime.Now;
                cnt++;
            }
            lock (Globals.ThisLock)
            {
                Globals.Countsl += sl;
                Globals.Countp += p;
                Globals.Countos += os;
                Globals.Countno += no;
                Globals.Countd += d;
                Globals.TotalCount += cnt;
            }
            return 1;
        }

        private static String RandomString(int strMin, int strMax)
        {
            const string randHold = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomString = "";
            for (int x = 0; x < Random.Next(strMin, strMax); ++x)
            {
                randomString += randHold.Substring(Random.Next(0, 62), 1);
            }
            return randomString;
        }

        private static void RawWrites()
        {
            var objConnect = new SqlConnection(Globals.StrPublisherConn);
            SqlCommand objCommand;

            string query = "insert into WriteTest (ID,RNDCHAR1,RNDCHAR2,RNDCHAR3,RNDCHAR4,RNDCHAR5,RNDCHAR6,RNDCHAR7,RNDCHAR8)" +
                           " VALUES(" +
                           "'" + Random.Next(1, 1000000000) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'," +
                           "'" + RandomString(1, 200) + "'" +
                           ")";
            try
            {
                objCommand = new SqlCommand(query, objConnect) { CommandTimeout = 3600 };
                objCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Errhandle.StopProcessing(e, query);
            }
        }

    }
}