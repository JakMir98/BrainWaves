using BrainWaves.ViewModels;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainWaves.Tests.ViewModelTests
{
    public class ScanViewModelTests
    {
        private ScanViewModel scanViewModel;
        [SetUp]
        public void Setup()
        {
            scanViewModel = new ScanViewModel();
        }

        [Test]
        public void ScanViewModel_ctorInit_HasTitle()
        {
            scanViewModel.Title.Length.Should().NotBe(0);
        }

        public void ScanViewModel_ctorInit_HasCommands()
        {
            using (new AssertionScope())
            {
                scanViewModel.GoToSettingsCommand.Should().NotBeNull();
                scanViewModel.ScanDevicesCommand.Should().NotBeNull();
                scanViewModel.StopScanningCommand.Should().NotBeNull();
            }
        }
    }
}
