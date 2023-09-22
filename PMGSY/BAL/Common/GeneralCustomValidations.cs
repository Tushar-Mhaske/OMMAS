


using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace PMGSY.BLL.Common
{



    /// <summary>
    /// function to check date format in DD/MM/YYYY
    /// </summary>

    public sealed class CheckDDMMYYYFormat : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            string[] strParams = Convert.ToString(value).Split('/');
            if (strParams.Length != 3 || strParams[0].Length != 2 || strParams[1].Length != 2 || strParams[2].Length != 4)
            {
                return false;
            }
            else return true;
        }

    }

    /// <summary>
    /// custom validation attribute to comapaire two dates
    /// </summary>
    public class IsDateAfter : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;
        CommonFunctions objCommon = new CommonFunctions();
        public IsDateAfter(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
                }

                var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                if (value != null)
                {
                    // Compare values
                    if (objCommon.GetStringToDateTime(value.ToString()) <= objCommon.GetStringToDateTime(propertyTestedValue.ToString()))
                    {
                        if (this.allowEqualDates)
                        {
                            return ValidationResult.Success;
                        }
                        if ((DateTime)value < (DateTime)propertyTestedValue)
                        {
                            return ValidationResult.Success;
                        }
                    }
                    else
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }

                if (value == null || !(value is DateTime))
                {
                    return ValidationResult.Success;
                }



                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdateafter"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
            yield return rule;
        }

    }

    public class IsDateBeforeOB : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;
        CommonFunctions objCommon = new CommonFunctions();
        public IsDateBeforeOB(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
                if (propertyTestedInfo == null)
                {
                    return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
                }

                var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                if (propertyTestedValue == null)
                {
                    return ValidationResult.Success;
                }

                //if (value == null || !(value is DateTime))
                //{
                //    return ValidationResult.Success;
                //}

                if (value != null)
                {
                    // Compare values
                    //if (objCommon.GetStringToDateTime(value.ToString()) >= Convert.ToDateTime(propertyTestedValue))

                    //new change done by Vikram 
                    if (objCommon.GetStringToDateTime(value.ToString()) >= objCommon.GetStringToDateTime(propertyTestedValue.ToString()))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }

                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isdatebeforeob"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            yield return rule;
        }

    }

    /// <summary>
    /// custom validation to check one of two fields is required 
    /// </summary>
    public class OneOfTwoRequired : ValidationAttribute, IClientValidatable
    {

        /*
         add this on js  page before doc.ready
            jQuery.validator.addMethod("oneoftworequired", function (value, element, param) {
            if ($('#firstId).val() == '' && $('#secondId).val() == '')
                return false;
            else
                return true;
        });

        jQuery.validator.unobtrusive.adapters.addBool("oneoftworequired");
     
         */


        private const string defaultErrorMessage = "{0} or {1} is required.";

        private string otherProperty;

        public OneOfTwoRequired(string otherProperty)
            : base(defaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }

            this.otherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, otherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                PropertyInfo otherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(otherProperty);

                if (otherPropertyInfo == null)
                {
                    return new ValidationResult(string.Format("Property '{0}' is undefined.", otherProperty));
                }

                var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                if (otherPropertyValue == null && value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "oneoftworequired"
            };

        }
    }


    /// <summary>
    /// custom validation to to apply required attribute based on the condition
    /// </summary>
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        protected RequiredAttribute _innerAttribute;

        public string DependentProperty { get; set; }
        public object TargetValue { get; set; }

        public bool AllowEmptyStrings
        {
            get
            {
                return _innerAttribute.AllowEmptyStrings;
            }
            set
            {
                _innerAttribute.AllowEmptyStrings = value;
            }
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _innerAttribute = new RequiredAttribute();
            DependentProperty = dependentProperty;
            TargetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                // get a reference to the property this validation depends upon
                var containerType = validationContext.ObjectInstance.GetType();
                var field = containerType.GetProperty(DependentProperty);

                if (field != null)
                {
                    // get the value of the dependent property
                    var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                    // trim spaces of dependent value
                    if (dependentValue != null && dependentValue is string)
                    {
                        dependentValue = (dependentValue as string).Trim();

                        if (!AllowEmptyStrings && (dependentValue as string).Length == 0)
                        {
                            dependentValue = null;
                        }
                    }

                    // compare the value against the target value
                    if ((dependentValue == null && TargetValue == null) ||
                        (dependentValue != null && (TargetValue.Equals("*") || dependentValue.Equals(TargetValue))))
                    {
                        // match => means we should try validating this field
                        if (!_innerAttribute.IsValid(value))
                            // validation failed - return an error
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };

            string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            // find the value on the control we depend on;
            // if it's a bool, format it javascript style 
            // (the default is True or False!)
            string targetValue = (TargetValue ?? "").ToString();
            if (TargetValue is bool)
                targetValue = targetValue.ToLower();

            rule.ValidationParameters.Add("dependentproperty", depProp);
            rule.ValidationParameters.Add("targetvalue", targetValue);

            yield return rule;
        }

        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext)
        {
            // build the ID of the property
            string depProp = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(DependentProperty);
            // unfortunately this will have the name of the current field appended to the beginning,
            // because the TemplateInfo's context has had this fieldname appended to it. Instead, we
            // want to get the context as though it was one level higher (i.e. outside the current property,
            // which is the containing object, and hence the same level as the dependent property.
            var thisField = metadata.PropertyName + "_";
            if (depProp.StartsWith(thisField))
                // strip it off again
                depProp = depProp.Substring(thisField.Length);
            return depProp;
        }
    }




    public class GeneralCustomValidations
    {
        public GeneralCustomValidations()
        {
        }

        /// <summary>
        /// function to check if empty field
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public string IsEmptyField(string paramName, string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue.ToString()))
                return ("Please enter " + paramName);

            else
                return ("");
        } // End of function IsEmptyField

        public string IsInvalidCharacter(string paramName, string paramValue)
        {
            string InvalidCharacters = "<>&;";
            char[] Charray = InvalidCharacters.ToCharArray();

            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters (<,>,&,;)");
            }

        } // End Of function Invalid Characters   


        public string IsAlphaNumeric(string paramName, string paramValue)
        {
            // if (!/^[A-Za-z0-9\/\-\s]+$/i.test()) {}
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9/-]");
            bool expStatus = objAlphaNumericPattern.IsMatch(paramValue);
            if (!expStatus)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters (use only '/','-')");
            }

        }

        public string CheckContactName(string paramName, string paramValue)
        {
            //var regExName = /[^\s][0-9a-zA-z]{1,20}/; //Name reg Ex Validation
            Regex objAlphaNumericPattern = new Regex("[^\\s][0-9a-zA-z]{1,20}");
            bool expStatus = objAlphaNumericPattern.IsMatch(paramValue);
            if (!expStatus)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters or name is too large (maximum 20 allowed)");
            }

        }

        public string CheckAddress(string paramName, string paramValue)
        {

            Regex objAlphaNumericPattern = new Regex("[^\n]+(?=[0-9a-zA-Z\\/\\s]{1,100})");
            bool expStatus = objAlphaNumericPattern.IsMatch(paramValue);
            if (!expStatus)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters or invalid number of digits (min 1 and max 100 digits allowed)");
            }

        }

        public string IsValidDate(string paramName, string paramValue)
        {

            try
            {
                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("fr-FR", true).DateTimeFormat; ;

                if (!(paramValue.Trim().Equals("")))
                {
                    Convert.ToDateTime(paramValue, dtfi);
                }
                return ("");
            }
            catch
            {
                return (paramName + " is invalid date");
            }
        } // End Of function Invalid date

        public string IsValidAmount(string paramName, string paramValue)
        {

            try
            {

                if (paramValue.Trim() != "")
                {
                    double num;
                    num = Convert.ToDouble(paramValue);

                    if (paramValue.Contains("."))
                    {
                        string[] idlist = paramValue.ToString().Trim().Split('.');


                        string beforDot = idlist[0].ToString().Trim();
                        string afterDot = idlist[1].ToString().Trim();

                        if (beforDot.ToString().Length > 11)
                        {
                            return (" Only eleven digits are allowed before . in " + paramName);

                        }
                        else
                        {
                            if (afterDot.ToString().Length > 2)
                            {
                                return (" Only 2 digits are allowed after . in " + paramName);

                            }
                            else
                            {
                                return ("");
                            }
                        }
                    }
                    else
                    {
                        if (paramValue.ToString().Length > 11)
                        {
                            return (" Improper " + paramName);
                        }
                    }




                } // end of not null
                return ("");
            }
            catch
            {
                return (paramName + " is invalid");
            }

        } // End of function to validate amount

        public string IsValidAmountInLakhs(string paramName, string paramValue)
        {

            try
            {

                double amount = Convert.ToDouble(paramValue.ToString().Trim());
                if (amount <= 0)
                {
                    return (paramName + " Must not Less than Zero.");
                }
                if (paramValue.Trim() != "")
                {
                    double num;
                    num = Convert.ToDouble(paramValue);

                    if (paramValue.Contains("."))
                    {
                        string[] idlist = paramValue.ToString().Trim().Split('.');


                        string beforDot = idlist[0].ToString().Trim();
                        string afterDot = idlist[1].ToString().Trim();

                        if (beforDot.ToString().Length > 3)
                        {
                            return (" Only three digits are allowed before . in " + paramName);

                        }
                        else
                        {
                            if (afterDot.ToString().Length > 2)
                            {
                                return (" Only 2 digits are allowed after . in " + paramName);

                            }
                            else
                            {
                                return ("");
                            }
                        }
                    }
                    else
                    {
                        if (paramValue.ToString().Length > 3)
                        {
                            return (" Invalid " + paramName);
                        }
                    }




                } // end of not null
                return ("");
            }
            catch
            {
                return (paramName + " is invalid");
            }

        } // End of function to validate amount

        public string IsValidAssetAmount(string paramName, string paramValue)
        {

            try
            {

                if (paramValue.Trim() != "")
                {
                    double num;
                    num = Convert.ToDouble(paramValue);

                    if (paramValue.Contains("."))
                    {
                        string[] idlist = paramValue.ToString().Trim().Split('.');


                        string beforDot = idlist[0].ToString().Trim();
                        string afterDot = idlist[1].ToString().Trim();

                        if (beforDot.ToString().Length > 9)
                        {
                            return (" Only nine digits are allowed before . in " + paramName);

                        }
                        else
                        {
                            if (afterDot.ToString().Length > 2)
                            {
                                return (" Only 2 digits are allowed after . in " + paramName);

                            }
                            else
                            {
                                return ("");
                            }
                        }
                    }
                    else
                    {
                        if (paramValue.ToString().Length > 12)
                        {
                            return (" Improper " + paramName);
                        }
                    }




                } // end of not null
                return ("");
            }
            catch
            {
                return (paramName + " is invalid");
            }

        } // End of function to validate amount

        public string IsExceedingStringLen(string paramName, string paramValue, int strlength)
        {

            if (paramValue.Length > strlength)
            {
                return (" Maximum " + strlength + " Characters are allowed in " + paramName);
            }
            else
            {
                return ("");
            }

        } // End of function to check maximum length 

        // Function to check whether given date is within month & year
        public int IsDateWithinMonthYear(string prDate, int month, int year)
        {
            string[] strDate = prDate.ToString().Split('/');
            int monthCompare = Convert.ToInt32(strDate[1]);  // retriving month value
            int YearCompare = Convert.ToInt32(strDate[2]); //retriving year value

            int value1 = YearCompare * 12 + monthCompare;
            int value2 = year * 12 + month;


            if (value1 == value2)
            {
                return 0;        // date is within selected month & year
            }
            else
            {
                return 1;
            }

        }

        //Function to check entered dates are greater than OBDate
        public int IsDateGreaterOBDate(string enteredDate, string obDate)
        {
            DateTime objenteredDate;
            DateTime objobDate;

            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("fr-FR", true).DateTimeFormat;

            objenteredDate = Convert.ToDateTime(enteredDate.Trim(), dtfi);
            objobDate = Convert.ToDateTime(obDate.Trim(), dtfi);


            if (objenteredDate >= objobDate)
            {
                return 0; // date is valid
            }
            else
            {
                return 1;
            }
        }//En

        //Function to validate email address
        public string IsValidEmail(string email)
        {

            if (string.IsNullOrEmpty(email))
            {
                return ("");
            }

            try
            {

                System.Net.Mail.MailAddress emailAddr = new System.Net.Mail.MailAddress(email);
                return ("");
            }
            catch
            {

                return ("Please enter valid Email address ");
            }
        }

        //Function to check numeric data type
        public string IsNumeric(string paramName, string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue))
            {
                return ("");
            }
            try
            {
                System.Int64.Parse(paramValue);
                return ("");
            }
            catch
            {
                return ("Only numeric characters are allowed in " + paramName);
            }

        }

        public string IsDropdownSelected(string paramName, int selectedValue)
        {

            if (selectedValue == 0)
            {
                return "Please select " + paramName;
            }
            else
            {
                return "";
            }
        }

        public string IsSepcialCharacter(string paramName, string paramValue, bool paramCheckForDot)
        {
            string InvalidCharacters = "!@#$%^&*()+=?|:;\"~`,[]{}";
            if (paramCheckForDot)
                InvalidCharacters += ".";
            char[] Charray = InvalidCharacters.ToCharArray();

            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters " + InvalidCharacters);
            }
        } // End Of function special characters Characters 

        public string IsDateInDDMMYYYYFormat(string paramName, string paramValue, char chSeparator)
        {
            string errorMsg = "";
            string[] strParams = paramValue.Split(chSeparator);
            if (strParams.Length != 3 || strParams[0].Length != 2 || strParams[1].Length != 2 || strParams[2].Length != 4)
            {
                errorMsg = paramName + " should be in dd" + chSeparator + "mm" + chSeparator + "yyyy format";
            }
            return errorMsg;
        }

        //can be used to check if descriptions, remarks, particulars, narrations etc are valid
        public string IsDescriptionValid(string paramName, string paramValue)
        {
            string InvalidCharacters = "`~!@#$%^*+;={}[]|\"'?";
            char[] Charray = InvalidCharacters.ToCharArray();

            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters " + InvalidCharacters);
            }
        }

        //function to check whether password length is not less than given passed number
        public string HasMinStringLength(string paramName, string paramValue, int strlength)
        {
            if (paramValue.Length < strlength)
            {
                return (paramName + "  must have minimum  " + strlength + " charactors ");
            }
            else
            {
                return ("");
            }

        }//End of function to check minimum password length 

        public string CheckConfirmPassword(string paramName1, string paramName2, string paramValue1, string paramValue2)
        {
            if (paramValue1.Equals(paramValue2) == false)
            {
                return (" Values in " + paramName1 + " and   " + paramName2 + " are not same ");
            }
            else
            {
                return ("");
            }
        }

        /*function to implement password policy 
                  1)Must have at least 6 characters
                  2)should not contain < and > 
                  
        */
        public string CheckPasswordPolicy(string paramName1, string paramValue1)
        {

            if (paramValue1.Contains("<") || paramValue1.Contains(">") || paramValue1.Length < 6)
            {
                return (paramName1 + " is invalid ");
            }
            else
            {
                return ("");
            }

        }

        //function to check whether entered birthdate is greater than current date[amol]
        public String IsBirthDateGreaterThanCurDate(string enteredDate, string curruntDate)
        {
            DateTime objenteredDate;
            DateTime curDate;

            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("fr-FR", true).DateTimeFormat;

            objenteredDate = Convert.ToDateTime(enteredDate.Trim(), dtfi);
            curDate = Convert.ToDateTime(curruntDate.Trim(), dtfi);


            if (objenteredDate > curDate)
            {
                return (" Entered Date should be less than current Date ");
            }
            else
            {
                return ("");
            }
        }

        //function to check for negative ammount [amol]
        public string IsPositive(string amount)
        {
            double value = Convert.ToDouble(amount);
            if (value <= 0)
            {
                return ("Improper Amount");
            }
            else
            {
                return ("");
            }


        }
        //function to check whether input contains space
        public string IsContainspace(string paramName, string paramValue)
        {
            if (paramValue.Contains(" "))
            {
                return (paramName + "is invalid");
            }
            else
            {
                return ("");
            }


        }

        //function to validate the reference number(authorization number)
        public string IsInvalidCharacterForRefNo(string paramName, string paramValue)
        {
            string InvalidCharacters = "`~!@#$%^&*_+=:;'{[}]|,\\?<>\"";
            char[] Charray = InvalidCharacters.ToCharArray();
            InvalidCharacters += "\\";
            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters");
            }

        } // End



        public int DifferenceBetweenDates(string dtLarge, string dtSmall)
        {

            string[] strDate1 = dtLarge.ToString().Split('/');
            string[] strDate2 = dtSmall.ToString().Split('/');


            int years = Convert.ToInt32(strDate2[2]) - Convert.ToInt32(strDate1[2]);

            if (Convert.ToInt32(strDate2[0]) < Convert.ToInt32(strDate1[1]) || (Convert.ToInt32(strDate2[0]) == Convert.ToInt32(strDate1[1]) && (Convert.ToInt32(strDate2[1]) < Convert.ToInt32(strDate1[0]))))
            {
                years = years - 1;
            }

            return years;


        }

        public string CheckUserName(string paramName, string paramValue)    //allows only A-Z','a-z' and '-'and '.'
        {

            string InvalidCharacters = "0123456789/`~!@#$%^&*_+=;:,'{[}]|\\?<>\"";

            char[] Charray = InvalidCharacters.ToCharArray();
            //InvalidCharacters += "\\";
            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters");
            }
        }

        public string CheckUserId(string paramName, string paramValue)    //allows only 'a-z','A-Z','0-9','-' and '.'
        {

            string InvalidCharacters = "/`~!@#$%^&*_+=;:,'{[}]|\\?<>\"";

            char[] Charray = InvalidCharacters.ToCharArray();
            //InvalidCharacters += "\\";
            int flag = 0;
            for (int i = 0; i < Charray.Length; i++)
            {
                int index = paramValue.IndexOf(Charray[i]);
                if (index > -1)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                return ("");
            }
            else
            {
                return (paramName + " contains invalid characters");
            }
        }

        public string IsNumericOfLength(string paramName, string paramValue, int minLength, int maxLength)
        {
            string strMinLength = "";
            string strMaxLength = "";
            string regEx = @"^\d{" + minLength + "," + maxLength + "}$";
            bool blnExpnStatus = false;
            if (minLength != -1)
            {
                strMinLength = minLength.ToString();
            }

            if (maxLength != -1)
            {
                strMaxLength = maxLength.ToString();
            }

            Regex regexString = new Regex(regEx);
            blnExpnStatus = regexString.IsMatch(paramValue);
            if (!blnExpnStatus)
            {
                return (paramName + " is invalid. " + String.Format("It should be Numeric Only with minimum {0} and maximum {1} digits.", minLength, maxLength));
            }
            else
            {
                return "";
            }
        }


    }

}
