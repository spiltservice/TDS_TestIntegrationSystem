/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace AIR.Common.Caching
{
    public interface ICacheManager<T>
    {
        T Get(string key);
        void Set(string key, T value);
        bool Contains(string key);
        bool Remove(string key);
    }
}