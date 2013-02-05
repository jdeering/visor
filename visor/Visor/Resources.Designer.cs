﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Visor {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Visor.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ***|ACCOUNT|Account
        ///	NUMBER|Account Number|1|0|10
        ///	TYPE|Account Type|2|5|99
        ///	LASTFMDATE|Last FM Date|3|3|null
        ///	OPENDATE|Open Date|5|3|null
        ///	CLOSEDATE|Close Date|6|3|null
        ///	BRANCH|Branch|7|5|9999
        ///	RESTRICT|Restricted Access|8|5|6
        ///	REFERENCE|Reference|9|0|20
        ///	WARNINGCODE:1|Warning 1 Code|10|5|999
        ///	WARNINGCODE:2|Warning 2 Code|10|5|999
        ///	WARNINGCODE:3|Warning 3 Code|10|5|999
        ///	WARNINGCODE:4|Warning 4 Code|10|5|999
        ///	WARNINGCODE:5|Warning 5 Code|10|5|999
        ///	WARNINGCODE:6|Warning 6 Code|10|5|999
        ///	WARNIN [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string db {
            get {
                return ResourceManager.GetString("db", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ABS|Returns the absolute value of an arithmetic expression|NUMBER,FLOAT,MONEY
        ///	Expression|An arithmetic expression|NUMBER,FLOAT,MONEY
        ///ANYWARNING|Boolean function that returns true if a warning exists|NUMBER
        ///	RECORD_TYPE|Account, Share, Loan, or Card Record to be searched|NULL
        ///	WARNING_CODE|Warning code that is to be searched for|NUMBER
        ///ANYSERVICE|Boolean function that returns true if a service code exists|NUMBER
        ///	RECORD_TYPE|Account, Share, Loan, or Card Record to be searched|NULL
        ///	SERVICE_CODE|Servi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string functions {
            get {
                return ResourceManager.GetString("functions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #INCLUDE|Includes a repGen file|#INCLUDE PROCEDURENAME
        ///ACCESS|ACCESS in conjunction with PREFERENCE designates the Preference Access record in the Account file|PREFERENCE ACCESS:MNEMONIC
        ///ACCOUNT|Designates account as record in the account file|ACCOUNT:MNEMONIC
        ///ACROSS|Designates how many labels across|ACROSS=3
        ///ACTIVITY|The purpose of the User Activity record is to store records of the user activity you select for tracking. ACTIVITY designates the User Activity record|ACTIVITY:CONSOLE
        ///AFTERLAST|Locator k [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string keywords {
            get {
                return ResourceManager.GetString("keywords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Toolbar {
            get {
                object obj = ResourceManager.GetObject("Toolbar", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PREVSYSTEMDATE|The last SYSTEMDATE for the current sym|DATE|
        ///SYSACTUALDATE|Actual calendar date|DATE|
        ///SYSACTUALTIME|Actual clock time on the server|NUMBER|
        ///SYSCLIENTNUMBER|Client Number of the Credit Union|NUMBER|
        ///SYSCONSOLENUM|Console number of user running this repgen|NUMBER|
        ///SYSSYMDIRECTORY|SYM Number|NUMBER|
        ///SYSTEMDATE|Current banking date|DATE|
        ///SYSMEMOMODE|Memo Mode|NUMBER|
        ///SYSUSERNUMBER|User number of current user|NUMBER|
        ///SYSWINDOWSLEVEL|Indicates windows level, 3=Windows, lower = console|NUM [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vars {
            get {
                return ResourceManager.GetString("vars", resourceCulture);
            }
        }
    }
}
