using Aspose.CAD;
using Aspose.CAD.FileFormats.Cad;
using Aspose.CAD.FileFormats.Cad.CadConsts;
using Aspose.CAD.FileFormats.Cad.CadObjects;
using Aspose.CAD.FileFormats.Cad.CadObjects.AttEntities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DWG
{
    class CAD
    {
        public void SearchTextInDWGAutoCADFile(string sourceFilePath, List<object> attrList)
        {
            // The path to the documents directory.
            //string MyDir = RunExamples.GetDataDir_DWGDrawings();
            //string sourceFilePath = MyDir + "search.dwg";
            // Load an existing DWG file as CadImage. 
            CadImage cadImage = (CadImage)Image.Load(sourceFilePath);
            // Search for text in the block section 
            //IterateCADInserts(cadImage);
            IterateCADInserts(cadImage, attrList);
            return;
        }

        public List<string> SearchInsertsAttributtesInDWGAutoCadFile(string sourceFilePath)
        {
            CadImage cadImage = (CadImage)Image.Load(sourceFilePath);
            return IterateCADInsertsATTRIB(cadImage);   
        }

        private static ArrayList IterateMTEXTNodes(CadImage cadImage)
        {
            ArrayList cadMTEXTList = new ArrayList();
            foreach (CadBlockEntity blockEntity in cadImage.BlockEntities.Values)
            {
                foreach (var entity in blockEntity.Entities)
                {
                    switch (entity.TypeName)
                    {
                        case CadEntityTypeName.MTEXT:
                            CadMText cadObjectMtext = (CadMText)entity;
                            cadMTEXTList.Add(cadObjectMtext);
                            break;
                    }
                }
            }
            return cadMTEXTList;
        }


        private static List<string> IterateCADInsertsATTRIB(CadImage cadImage)
        {
            List<string> items = new List<string>();
            foreach (CadBlockEntity blockEntity in cadImage.BlockEntities.Values)
            {
                foreach (var entity in blockEntity.Entities)
                {
                    switch (entity.TypeName)
                    {
                        case CadEntityTypeName.INSERT:
                            CadInsertObject childInsertObject = (CadInsertObject)entity;
                            foreach (var attributes in childInsertObject.ChildObjects)
                            {
                                if (attributes.TypeName == CadEntityTypeName.ATTRIB)
                                {
                                    CadAttrib attribute = (CadAttrib)attributes;
                                    string attr = attribute.DefinitionTagString.Value;
                                    items.Add(attr);
                                }
                            }
                            break;
                    }
                }
            }
            return items;
        }

        private static void IterateCADInserts(CadImage cadImage, List<object> attrList)
        {
            int map_id = Form1.map_id;
            int sys_id = Form1.sys_id;
            int grp_id = Form1.grp_id;
            
            foreach (CadBlockEntity blockEntity in cadImage.BlockEntities.Values)
            {
                foreach (var entity in blockEntity.Entities)
                {

                    switch (entity.TypeName)
                    {
                        case CadEntityTypeName.INSERT:
                            CadInsertObject childInsertObject = (CadInsertObject)entity;
                            ArrayList mtext = IterateMTEXTNodes(cadImage);
                            string BTNdsc = string.Empty;
                            foreach (CadMText text in mtext)
                            {
                                double ABSposX = Math.Abs(childInsertObject.InsertionPoint.X - text.InsertionPoint.X);
                                double ABSposY = Math.Abs(childInsertObject.InsertionPoint.Y - text.InsertionPoint.Y);
                                if (ABSposX <= 40 && ABSposY <= 40)
                                {
                                    BTNdsc = text.Text;
                                }
                            }
                            foreach (var attributes in childInsertObject.ChildObjects)
                            {
                                if (attributes.TypeName == CadEntityTypeName.ATTRIB)
                                {
                                    CadAttrib at = (CadAttrib)attributes;
                                    foreach (string attrName in attrList)
                                    {
                                        if (at.DefinitionTagString.Value == attrName)
                                        {
                                            BTNdsc = at.DefaultText;
                                        }
                                    }
                                }
                            }
                            Database db = new Database();
                            db.InsertMap_zne(map_id,sys_id,grp_id, childInsertObject.InsertionPoint.X, childInsertObject.InsertionPoint.Y, childInsertObject.RotationAngle, childInsertObject.Name, BTNdsc);
                            db.Dispose();
                            break;
                    }
                }
            }
        }
    }
}
