﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace ExampleAPI.Models.ExampleXPOModel
{

    public partial class ExampleObject : XPObject
    {
        string fFirstName;
        public string FirstName
        {
            get { return fFirstName; }
            set { SetPropertyValue<string>(nameof(FirstName), ref fFirstName, value); }
        }
        string fLastName;
        public string LastName
        {
            get { return fLastName; }
            set { SetPropertyValue<string>(nameof(LastName), ref fLastName, value); }
        }
        string fPhone;
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>(nameof(Phone), ref fPhone, value); }
        }
    }

}
