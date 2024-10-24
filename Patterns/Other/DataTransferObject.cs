﻿using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Description : https://en.wikipedia.org/wiki/Data_transfer_object
// Source      : https://www.codeproject.com/Articles/8824/C-Data-Transfer-Object

namespace Patterns.Other
{
    /// 
    /// This is the base class for all DataTransferObjects.
    /// 
    public abstract class DataTransferObjects
    {
    }

    #region Data Transfer Object Serializer Helper Class
    /// 
    /// Summary description for DTOSerializerHelper.
    /// 
    public class DtoSerializerHelper
    {
        /// 
        /// Creates xml string from given dto.
        /// 
        /// DTO
        /// XML
        public static string SerializeDto(DataTransferObjects dto)
        {
            XmlSerializer xmlSer = new XmlSerializer(dto.GetType());
            StringWriter sWriter = new StringWriter();
            // Serialize the dto to xml.
            xmlSer.Serialize(sWriter, dto);
            // Return the string of xml.
            return sWriter.ToString();
        }

        /// 
        /// Deserializes the xml into a specified data transfer object.
        /// 
        /// string of xml
        /// type of dto
        /// DTO
        public static DataTransferObjects DeserializeXml(string xml, DataTransferObjects dto)
        {
            XmlSerializer xmlSer = new XmlSerializer(dto.GetType());
            // Read the XML.
            StringReader sReader = new StringReader(xml);
            
            // Cast the deserialized xml to the type of dto.
            DataTransferObjects retDto = (DataTransferObjects)xmlSer.Deserialize(sReader)!;
            
            // Return the data transfer object.
            return retDto;
        }

    }
    #endregion

    #region Derived Data Transfer Object Example

    /// 
    /// Summary description for the DemoDTO.
    /// 
    public class DemoDto : DataTransferObjects
    {
        // Variables encapsulated by class (private).

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoId { get; set; } = "";

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoName { get; set; } = "";

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoProgrammer { get; set; } = "";
    }
    #endregion

    #region Exmaple
    /// 
    /// Summary description for DemoClass.
    /// 
    public class DataTransferObjectExample
    {
        public void StartDemo()
        {
            this.ProcessDemo();
        }

        private DemoDto CreateDemoDto()
        {
            DemoDto dto = new DemoDto
            {
                DemoId = "1",
                DemoName = "Data Transfer Object Demonstration Program",
                DemoProgrammer = "Kenny Young"
            };

            return dto;
        }

        private void ProcessDemo()
        {
            DemoDto dto = this.CreateDemoDto();

            // Write the deserialized dto values.
            Console.WriteLine("Not Serialized DTO");
            Console.WriteLine("=======================");
            Console.WriteLine("\r");
            Console.WriteLine("DemoId         : " + dto.DemoId);
            Console.WriteLine("Demo Name      : " + dto.DemoName);
            Console.WriteLine("Demo Programmer: " + dto.DemoProgrammer);
            Console.WriteLine("\r");

            // Serialize the dto to xml.
            string strXml = DtoSerializerHelper.SerializeDto(dto);

            // Write the serialized dto as xml.
            Console.WriteLine();
            Console.WriteLine("Serialized DTO");
            Console.WriteLine("=======================");
            Console.WriteLine("\r");
            Console.WriteLine(strXml);
            Console.WriteLine("\r");

            // Deserialize the xml to the data transfer object.
            DemoDto desDto =
              (DemoDto)DtoSerializerHelper.DeserializeXml(strXml,
              new DemoDto());

            Assert.AreEqual(dto.DemoId, desDto.DemoId);
            Assert.AreEqual(dto.DemoName, desDto.DemoName);
            Assert.AreEqual(dto.DemoProgrammer, desDto.DemoProgrammer);

            // Write the deserialized dto values.
            Console.WriteLine("Deseralized DTO");
            Console.WriteLine("=======================");
            Console.WriteLine("\r");
            Console.WriteLine("DemoId         : " + desDto.DemoId);
            Console.WriteLine("Demo Name      : " + desDto.DemoName);
            Console.WriteLine("Demo Programmer: " + desDto.DemoProgrammer);
            Console.WriteLine("\r");
        }
    }
    #endregion
}
