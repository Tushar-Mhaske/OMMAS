using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  PMGSY.Common
{
    public class AmountToWord
    {
        public string RupeesToWord(string MyNumber)
        {

            string Temp;
            string Rupees;
            string Paisa = "";
            //----string DecimalPlace;
            int DecimalPlace;
            int iCount;
            string Hundreds;
            string Words = "";
            //----string[] place = new string[10];
            string[] place = { " Thousand ", "", " Lakh ", "", " Crore ", "", " Arab ", "", " Kharab " };
            place[0] = " Thousand ";
            place[2] = " Lakh ";

            place[4] = " Crore ";
            place[6] = " Arab ";
            place[8] = " Kharab ";
            // ERROR: Not supported in C#: OnErrorStatement 
            // Convert MyNumber to a string, trimming extra spaces. 
            MyNumber = MyNumber.ToString().Trim();
            if (MyNumber.Equals("0.00") || MyNumber.Equals(".00") || MyNumber.Equals("0"))
            {
                string valueInWords; 
                valueInWords = "Rupees Zero Only";
                return valueInWords;
            }
           


            //Find decimal place. 
            DecimalPlace = MyNumber.ToString().IndexOf(".");
            //Rupees supplied is single digit only then
            if (DecimalPlace == 1)
            {
                MyNumber = "0" + MyNumber;
                DecimalPlace = 2;
            }
            // If we find decimal place... 
            //if (MyNumber.ToString().Contains("."))
            if (DecimalPlace > 0)
            {
                // Convert Paisa 
                //----Temp = Strings.Left(Strings.Mid(MyNumber, DecimalPlace + 1) + "00", 2);
                //----Paisa = " and " + ConvertTens(Temp) + " Paisa";

                // Temp = MyNumber.ToString().Substring(DecimalPlace + 1, 2);
                Temp = MyNumber.ToString().Substring(DecimalPlace + 1, 2);
                string paise = ConvertTens(Temp);
                if (paise != "")
                {
                    Paisa = " and " + paise + " Paisa";
                }

                // Strip off paisa from remainder to convert. 
                //----MyNumber = Strings.Trim(Strings.Left(MyNumber, DecimalPlace - 1));
                char[] dot = { '.' };
                string[] tempNumWithoutDot;
                tempNumWithoutDot = MyNumber.ToString().Split(dot);
                MyNumber = tempNumWithoutDot[0];
            }

            //=============================================================== 
            string TM;

            //----TM = Strings.Right(MyNumber, 2);
            TM = MyNumber.ToString().Substring(DecimalPlace - 2);
            // If MyNumber between Rs.1 To 99 Only. 
            if ((MyNumber.ToString().Length > 0) && (MyNumber.ToString().Length <= 2))
            {
                if (TM.Length == 1)
                {
                    Words = ConvertDigit(TM);
                    string valueInWords;
                    valueInWords = "Rupees " + Words + Paisa + " Only";
                    //----RupeesToWord = "Rupees " + Words + Paisa + " Only";

                    return valueInWords; // TODO: might not be correct. Was : Exit Function 
                }

                else
                {
                    if (TM.Length == 2)
                    {
                        Words = ConvertTens(TM);
                        string valueInWords;
                        valueInWords = "Rupees " + Words + Paisa + " Only";
                        //----RupeesToWord = "Rupees " + Words + Paisa + " Only";
                        return valueInWords; // TODO: might not be correct. Was : Exit Function 

                    }
                }
            }


            // Convert last 3 digits of MyNumber to ruppees in word. 
            Hundreds = ConvertHundreds(MyNumber.ToString().Substring(MyNumber.Length - 3));
            // Strip off last three digits 
            //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 3);
            MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 3);

            iCount = 0;
            while (MyNumber != "")
            {
                //Strip last two digits 
                //---- Temp = Strings.Right(MyNumber, 2);
                if (MyNumber.Length == 1)
                    MyNumber = "0" + MyNumber;
                Temp = MyNumber.ToString().Substring(MyNumber.ToString().Length - 2);

                if (MyNumber.ToString().Length == 1)
                {


                    if (Words.Trim() == "Thousand" | Words.Trim() == "Lakh Thousand" | Words.Trim() == "Lakh" | Words.Trim() == "Crore" | Words.Trim() == "Crore Lakh Thousand" | Words.Trim() == "Arab Crore Lakh Thousand" | Words.Trim() == "Arab" | Words.Trim() == "Kharab Arab Crore Lakh Thousand" | Words.Trim() == "Kharab")
                    {

                        Words = ConvertDigit(Temp) + place[iCount];
                        //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 1);
                        MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 1);
                    }

                    else
                    {

                        Words = ConvertDigit(Temp) + place[iCount] + Words;
                        //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 1);
                        MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 1);

                    }
                }
                else
                {

                    if (Words.Trim() == "Thousand" | Words.Trim() == "Lakh Thousand" | Words.Trim() == "Lakh" | Words.Trim() == "Crore" | Words.Trim() == "Crore Lakh Thousand" | Words.Trim() == "Arab Crore Lakh Thousand" | Words.Trim() == "Arab")
                    {


                        Words = ConvertTens(Temp) + place[iCount];


                        //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 2);
                        MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 2);
                    }
                    else
                    {

                        //================================================================= 
                        // if only Lakh, Crore, Arab, Kharab 

                        string resultConversion = ConvertTens(Temp);
                        resultConversion = resultConversion.Trim() + place[iCount];

                        if (resultConversion == "Lakh" | resultConversion == "Crore" | resultConversion == "Arab")
                        {

                            //---- Words = Words;
                            //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 2);
                            MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 2);
                        }
                        else
                        {
                            //Words = ConvertTens(Temp) + place[iCount] + Words;//commented on 27-02-2023
                            Words = Temp.Equals("00") ? Words : ConvertTens(Temp) + place[iCount] + Words;//Added on 27-02-2023
                            //----MyNumber = Strings.Left(MyNumber, Strings.Len(MyNumber) - 2);
                            //MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 2);
                            MyNumber = MyNumber.ToString().Substring(0, MyNumber.ToString().Length - 2);
                        }

                    }
                }

                iCount = iCount + 2;
            }
            return "Rupees " + Words + Hundreds + Paisa + " Only";

        }

        // Conversion for hundreds 
        //***************************************** 
        private string ConvertHundreds(string MyNumber)
        {
            string Result = "";

            // Exit if there is nothing to convert. 
            if (Convert.ToInt32(MyNumber.ToString()) == 0)
                return ""; // TODO: might not be correct. Was : Exit Function 


            // Append leading zeros to number. 
            //----MyNumber = Strings.Right("000" + MyNumber, 3);
            //MyNumber = MyNumber.ToString()+"000";
            MyNumber = MyNumber.ToString().Substring(MyNumber.ToString().Length - 3);

            // Do we have a hundreds place digit to convert? 
            if (MyNumber.ToString().Substring(0, 1) != "0")
            {
                Result = ConvertDigit(MyNumber.ToString().Substring(0, 1)) + " Hundred ";
            }

            // Do we have a tens place digit to convert? 
            if (MyNumber.ToString().Substring(1, 2) != "0")
            {
                Result = Result + ConvertTens(MyNumber.ToString().Substring(1));
            }
            else
            {
                // If not, then convert the ones place digit. 
                Result = Result + ConvertDigit(MyNumber.ToString().Substring(1));
            }

            return Result.ToString().Trim();
        }

        // Conversion for tens 
        //***************************************** 
        private string ConvertTens(string MyTens)
        {
            string Result = "";

            // Is value between 10 and 19? 
            if (Convert.ToInt32(MyTens.ToString().Substring(MyTens.ToString().Length - 2, 1)) == 1)
            {
                switch (Convert.ToInt32(MyTens.ToString()))
                {
                    case 10:
                        Result = "Ten";
                        break;
                    case 11:
                        Result = "Eleven";
                        break;
                    case 12:
                        Result = "Twelve";
                        break;
                    case 13:
                        Result = "Thirteen";
                        break;
                    case 14:
                        Result = "Fourteen";
                        break;
                    case 15:
                        Result = "Fifteen";
                        break;
                    case 16:
                        Result = "Sixteen";
                        break;
                    case 17:
                        Result = "Seventeen";
                        break;
                    case 18:
                        Result = "Eighteen";
                        break;
                    case 19:
                        Result = "Nineteen";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // .. otherwise it's between 20 and 99. 
                switch (Convert.ToInt32(MyTens.ToString().Substring(MyTens.Length - 2, 1)))
                {
                    case 2:
                        Result = "Twenty ";
                        break;
                    case 3:
                        Result = "Thirty ";
                        break;
                    case 4:
                        Result = "Forty ";
                        break;
                    case 5:
                        Result = "Fifty ";
                        break;
                    case 6:
                        Result = "Sixty ";
                        break;
                    case 7:
                        Result = "Seventy ";
                        break;
                    case 8:
                        Result = "Eighty ";
                        break;
                    case 9:
                        Result = "Ninety ";
                        break;
                    default:
                        break;
                }

                // Convert ones place digit. 
                Result = Result + ConvertDigit(MyTens.ToString().Substring(MyTens.ToString().Length - 1, 1));
            }
            return Result;

        }

        private string ConvertDigit(string MyDigit)
        {
            string functionReturnValue = "";
            switch (Convert.ToInt32(MyDigit))
            {
                case 1:
                    functionReturnValue = "One";
                    break;
                case 2:
                    functionReturnValue = "Two";
                    break;
                case 3:
                    functionReturnValue = "Three";
                    break;
                case 4:
                    functionReturnValue = "Four";
                    break;
                case 5:
                    functionReturnValue = "Five";
                    break;
                case 6:
                    functionReturnValue = "Six";
                    break;
                case 7:
                    functionReturnValue = "Seven";
                    break;
                case 8:
                    functionReturnValue = "Eight";
                    break;
                case 9:
                    functionReturnValue = "Nine";
                    break;
                default:
                    functionReturnValue = "";
                    break;
            }
            return functionReturnValue;
        } 
    }
}
