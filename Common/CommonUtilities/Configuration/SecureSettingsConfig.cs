/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace CommonUtilities.Configuration
{
    public class SecureSettingsConfig : ConfigurationSection
    {
        //Index into the FTPSettings by name.
        //Note: Setting name to "" allows me to skip the outer <SecureSettings><SecureSettings> node
        [ConfigurationProperty("",
            IsDefaultCollection = true, IsRequired = true)]
        public SecureSettings SecureSettings
        {
            get
            {
                SecureSettings secureSettingsCollection =
                (SecureSettings)base[""];
                return secureSettingsCollection;
            }
        }
    }
}
