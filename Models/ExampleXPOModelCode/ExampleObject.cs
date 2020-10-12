using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace ExampleAPI.Models.ExampleXPOModel
{

    public partial class ExampleObject
    {
        public ExampleObject(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
