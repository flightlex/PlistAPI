﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlistAPI.Tests.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PlistAPI.Tests.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
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
        ///   Ищет локализованную строку, похожую на &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;!DOCTYPE plist PUBLIC &quot;-//Apple//DTD PLIST 1.0//EN&quot; &quot;http://www.apple.com/DTDs/PropertyList-1.0.dtd&quot;&gt;
        ///&lt;plist version=&quot;1.0&quot;&gt;
        ///	&lt;dict&gt;
        ///		&lt;k&gt;SomeInt&lt;/k&gt;
        ///		&lt;i&gt;13371337&lt;/i&gt;
        ///
        ///		&lt;key&gt;Person&lt;/key&gt;
        ///		&lt;dict&gt;
        ///			&lt;key&gt;FirstName&lt;/key&gt;
        ///			&lt;string&gt;John&lt;/string&gt;
        ///			&lt;key&gt;LastName&lt;/key&gt;
        ///			&lt;s&gt;Public&lt;/s&gt;
        ///			&lt;key&gt;Address&lt;/key&gt;
        ///			&lt;dict&gt;
        ///				&lt;key&gt;Street&lt;/key&gt;
        ///				&lt;s&gt;123 Anywhere St.&lt;/s&gt;
        ///				&lt;key&gt;City&lt;/key&gt;
        ///				&lt;string&gt;Some Town&lt;/string&gt;
        ///				&lt;key&gt;State&lt;/key&gt;
        ///				&lt;s&gt;CA&lt;/s [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string ComplicatedPlist {
            get {
                return ResourceManager.GetString("ComplicatedPlist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;!DOCTYPE plist PUBLIC &quot;-//Apple Computer//DTD PLIST 1.0//EN&quot; &quot;http://www.apple.com/DTDs/PropertyList-1.0.dtd&quot;&gt;
        ///&lt;plist version=&quot;1.0&quot;&gt;
        ///	&lt;dict&gt;
        ///		&lt;key&gt;Simple&lt;/key&gt;
        ///		&lt;string&gt;Plist&lt;/string&gt;
        ///		&lt;key&gt;IntValue&lt;/key&gt;
        ///		&lt;integer&gt;696969&lt;/integer&gt;
        ///	&lt;/dict&gt;
        ///&lt;/plist&gt;.
        /// </summary>
        internal static string SimplePlist {
            get {
                return ResourceManager.GetString("SimplePlist", resourceCulture);
            }
        }
    }
}