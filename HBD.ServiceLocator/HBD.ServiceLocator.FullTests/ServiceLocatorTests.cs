﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBD.ServiceLocator.FullTests
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void Access_The_Current_Without_Initilize()
        {
            Action a = () => ServiceLocator.Current.GetInstance(null);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void Initilize_Null_Instace()
        {
            Action a = () => ServiceLocator.SetServiceLocator((IServiceLocator)null);
            a.ShouldThrow<ArgumentNullException>();

            Action b = () => ServiceLocator.SetServiceLocator((Func<IServiceLocator>)null);
            b.ShouldThrow<ArgumentNullException>();

            Action c = () => ServiceLocator.SetServiceLocator((Func<CompositionContainer>)null);
            c.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Lazy_Initialize_Composition()
        {
            var count = 0;
            ServiceLocator.SetServiceLocator(() =>
             {
                 count++;

                 if (count <= 1)
                     return null;
                 return new CompositionContainer(new AggregateCatalog());
             });

            Action a = () => ServiceLocator.Current.GetAllInstances(null);
            a.ShouldThrow<Exception>();

            Action b = () => ServiceLocator.Current.GetAllInstances(typeof(object));
            b.ShouldNotThrow();
        }

        [TestMethod]
        public void Lazy_Initialize_Service()
        {
            var count = 0;
            ServiceLocator.SetServiceLocator(() =>
            {
                count++;

                if (count <= 1)
                    return null;
                return new MefServiceLocator(new CompositionContainer(new AggregateCatalog()));
            });

            Action a = () => ServiceLocator.Current.GetAllInstances(null);
            a.ShouldThrow<Exception>();

            Action b = () => ServiceLocator.Current.GetAllInstances(typeof(object));
            b.ShouldNotThrow();
        }
    }
}
