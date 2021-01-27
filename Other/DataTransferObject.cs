using System;
using System.Xml.Serialization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Description : https://en.wikipedia.org/wiki/Data_transfer_object
// Source      : https://www.codeproject.com/Articles/8824/C-Data-Transfer-Object

namespace Other.DataTransferObject
{
    /// 
    /// This is the base class for all DataTransferObjects.
    /// 
    public abstract class DTO
    {
        public DTO()
        {
        }
    }

    #region Data Transfer Object Serializer Helper Class
    /// 
    /// Summary description for DTOSerializerHelper.
    /// 
    public class DTOSerializerHelper
    {
        public DTOSerializerHelper()
        {
        }

        /// 
        /// Creates xml string from given dto.
        /// 
        /// DTO
        /// XML
        public static string SerializeDTO(DTO dto)
        {
            try
            {
                XmlSerializer xmlSer = new XmlSerializer(dto.GetType());
                StringWriter sWriter = new StringWriter();
                // Serialize the dto to xml.
                xmlSer.Serialize(sWriter, dto);
                // Return the string of xml.
                return sWriter.ToString();
            }
            catch (Exception ex)
            {
                // Propogate the exception.
                throw ex;
            }
        }

        /// 
        /// Deserializes the xml into a specified data transfer object.
        /// 
        /// string of xml
        /// type of dto
        /// DTO
        public static DTO DeserializeXml(string xml, DTO dto)
        {
            try
            {
                XmlSerializer xmlSer = new XmlSerializer(dto.GetType());
                // Read the XML.
                StringReader sReader = new StringReader(xml);
                // Cast the deserialized xml to the type of dto.
                DTO retDTO = (DTO)xmlSer.Deserialize(sReader);
                // Return the data transfer object.
                return retDTO;
            }
            catch (Exception ex)
            {
                // Propogate the exception.
                throw ex;
            }
        }

    }
    #endregion

    #region Derived Data Transfer Object Example

    /// 
    /// Summary description for the DemoDTO.
    /// 
    public class DemoDTO : DTO
    {
        // Variables encapsulated by class (private).
        private string demoId = "";
        private string demoName = "";
        private string demoProgrammer = "";

        public DemoDTO()
        {
        }

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoId
        {
            get
            {
                return this.demoId;
            }
            set
            {
                this.demoId = value;
            }
        }

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoName
        {
            get
            {
                return this.demoName;
            }
            set
            {
                this.demoName = value;
            }
        }

        ///Public access to the DemoId field.
        ///String
        [XmlElement(IsNullable = true)]
        public string DemoProgrammer
        {
            get
            {
                return this.demoProgrammer;
            }
            set
            {
                this.demoProgrammer = value;
            }
        }

    }
    #endregion

    #region Exmaple
    /// 
    /// Summary description for DemoClass.
    /// 
    public class DataTransferObjectExample
    {
        public DataTransferObjectExample()
        {
        }

        public void StartDemo()
        {
            this.ProcessDemo();
        }

        private DemoDTO CreateDemoDto()
        {
            DemoDTO dto = new DemoDTO();

            dto.DemoId = "1";
            dto.DemoName = "Data Transfer Object Demonstration Program";
            dto.DemoProgrammer = "Kenny Young";

            return dto;
        }

        private void ProcessDemo()
        {
            DemoDTO dto = this.CreateDemoDto();

            // Serialize the dto to xml.
            string strXml = DTOSerializerHelper.SerializeDTO(dto);

            // Write the serialized dto as xml.
            Console.WriteLine();
            Console.WriteLine("Serialized DTO");
            Console.WriteLine("=======================");
            Console.WriteLine("\r");
            Console.WriteLine(strXml);
            Console.WriteLine("\r");

            // Deserialize the xml to the data transfer object.
            DemoDTO desDto =
              (DemoDTO)DTOSerializerHelper.DeserializeXml(strXml,
              new DemoDTO());

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
