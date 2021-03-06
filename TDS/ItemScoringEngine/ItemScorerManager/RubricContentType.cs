/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Describes how the rubric information is included - as a path to the rubric file or the content is provided as a string
    /// </summary>
    public enum RubricContentType
    {
        /// <summary>
        /// URI path to the rubric provided
        /// </summary>
        Uri, 
        /// <summary>
        /// Contents of the rubric is provided as a string
        /// </summary>
        ContentString
    };
}