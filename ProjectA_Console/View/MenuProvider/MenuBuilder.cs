using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
namespace ProjectA_Console.View.MenuProvider
{
    class MenuBuilder
    {
        public Menu Build(string menuSourcePath, Controller.Controller controller)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(menuSourcePath);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"{menuSourcePath} файлы табылмады! ");
                Console.ReadKey();
                throw;
            }
            XmlElement xRoot = xDoc.DocumentElement;
            Menu rootMenu = new();
            Debug.Assert(xRoot != null, nameof(xRoot) + " != null");
            List<MenuItem> menuItems = new List<MenuItem>();
            foreach (XmlNode node in xRoot)
            {
                menuItems.Add(CreateMenuItem(node, controller));
            }

            rootMenu.MenuItems = new ObservableCollection<MenuItem>(menuItems);
            return rootMenu;
        }

        private MenuItem CreateMenuItem(XmlNode node, Controller.Controller controller)
        {
            MenuItem menuItem = new MenuItem();
            Debug.Assert(node.Attributes != null, $"node.Attributes != null ({node.InnerText})");

            menuItem.Title = node.Attributes.GetNamedItem("Name").Value;

            if (node.HasChildNodes)
            {
                menuItem.IsMenu = true;
                List<MenuItem> innerMenuItems = new List<MenuItem>();
                foreach (XmlNode innerNode in node)
                {
                    if(innerNode is not XmlText)
                        innerMenuItems.Add(CreateMenuItem(innerNode, controller));
                }

                menuItem.InnerMenu = new Menu()
                {
                    MenuItems = new ObservableCollection<MenuItem>(innerMenuItems)
                };
            }
            else
            {
                menuItem.IsMenu = false;
                if (node.Attributes != null)
                {
                    XmlNode commandName = node.Attributes.GetNamedItem("Command");
                    var methodInfo = controller.GetType().GetMethod(commandName.Value);
                    Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

                    menuItem.Action = (Action) Delegate.CreateDelegate(typeof(Action), controller, methodInfo);
                }
            }

            return menuItem;
        }
    }
}