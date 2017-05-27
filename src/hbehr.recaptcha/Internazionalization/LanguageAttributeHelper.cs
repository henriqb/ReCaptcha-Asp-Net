/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 - 2017 Henrique B. Behr
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace hbehr.recaptcha.Internazionalization
{
    public static class LanguageAttributeHelper
    {
        public static string GetLanguage(this ReCaptchaLanguage language)
        {
            if (language == ReCaptchaLanguage.Auto)
            {
                var newLanguage = GetLanguageByCulture(CultureInfo.CurrentUICulture);
                return GetLanguage(newLanguage.GetValueOrDefault());
            }
            var attribute = language.GetType().GetMember(language.ToString()).Select(m => m.GetCustomAttribute<LanguageAttribute>()).FirstOrDefault() ?? new LanguageAttribute(string.Empty);
            return attribute.Value;
        }

        public static ReCaptchaLanguage? GetLanguageByCulture(string culture)
        {                       
            var member = typeof(ReCaptchaLanguage).GetMembers()
                .Where(m => m.GetCustomAttribute<LanguageAttribute>() != null && m.GetCustomAttribute<LanguageAttribute>().Value.Equals(culture, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();            
            return ConvertLangType(member);
        }

        public static ReCaptchaLanguage? GetLanguageByCulture(CultureInfo culture)
        {
            return GetLanguageByCulture((culture.Parent != CultureInfo.InvariantCulture ? culture.Parent : culture).ToString());
        }

        private static ReCaptchaLanguage? ConvertLangType(MemberInfo memberInfo)
        {
            return memberInfo != null ?        
                (ReCaptchaLanguage?)((FieldInfo)memberInfo).GetValue(memberInfo.Name)
                : null;
        }

        private static T GetCustomAttribute<T>(this MemberInfo type)
        {
            return (T)type.GetCustomAttributes(typeof (T), true).FirstOrDefault();
        }
    }
}