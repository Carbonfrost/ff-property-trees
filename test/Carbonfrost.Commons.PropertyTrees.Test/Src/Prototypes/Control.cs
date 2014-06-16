//
// - Control.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Shared.Runtime;

[assembly: Provides(typeof(Prototypes.Control))]

#pragma warning disable 3003
#pragma warning disable 3008

namespace Prototypes {

    [Composable]
    public abstract class Control : IAddChild<Control> {

        private readonly IList<Control> controls = new Collection<Control>();

        public IList<Control> Controls {
            get { return controls; }
        }

        public virtual void AddChild(Control item) {
            controls.Add(item);
        }

        public void AddChild(object item) {
            AddChild((Control) item);
        }

        public void AddText(string text) {}

        public int _Top { get; set; }
        public int _Left { get; set; }
        public Canvas _Canvas { get; set; }
    }

    [Provider(typeof(Control), Name = "compose")]
    public class ContainerControl : Control {
    }

    [Provider(typeof(Control), Name = "p")]
    public class Paragraph : Control {}

    [Provider(typeof(Control), Name = "textbox")]
    public class TextBox : Control {}

    [Provider(typeof(Control), Name = "canvas")]
    public class Canvas : Control {

        [Extender]
        public void SetTop(Control control, int top) {
            control._Top = top;
            control._Canvas = this;
        }
    }

    public static class ControlExtensions {

        [Extender]
        public static void SetLeft(this Control control, int left) {
            control._Left = left;
        }
    }

}
