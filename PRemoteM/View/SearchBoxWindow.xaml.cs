﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PRM.ViewModel;

namespace PRM.View
{
    /// <summary>
    /// SearchBoxWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchBoxWindow : Window
    {
        private readonly VmSearchBox _vmSearchBox = null;


        public SearchBoxWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            _vmSearchBox = new VmSearchBox();
            DataContext = _vmSearchBox;
            Loaded += (sender, args) =>
            {
                HideMe();
                Deactivated += (sender1, args1) => { Dispatcher.Invoke(HideMe); };
                KeyDown += (sender1, args1) =>
                {
                    if (args1.Key == Key.Escape) HideMe();
                };
            };
        }

        private readonly object _closeLocker = new object();
        private bool _isHidden = false;
        private void HideMe()
        {
            if (_isHidden == false)
                lock (_closeLocker)
                {
                    if (_isHidden == false)
                    {
                        this.Visibility = Visibility.Hidden;
                        _vmSearchBox.DispNameFilter = "";
                        _isHidden = true;
                    }
                }
        }

        public void ShowMe()
        {
            if (_isHidden == true)
                lock (_closeLocker)
                {
                    if (_isHidden == true)
                    {
                        this.Visibility = Visibility.Visible;

                        this.Activate();
                        this.Topmost = true;  // important
                        this.Topmost = false; // important
                        this.Focus();         // important
                        TbKeyWord.Focus();

                        _isHidden = false;
                    }
                }
        }










        private void WindowHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }




        private readonly object _keyDownLocker = new object();
        private void TbKeyWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    HideMe();
                    break;
                case Key.Enter:
                    {
                        lock (_closeLocker)
                        {
                            var i = _vmSearchBox.SelectedServerTextIndex;
                            var j = _vmSearchBox.DispServerList.Count;
                            var s = _vmSearchBox.DispServerList[i];
                            if (i < j && i >= 0)
                            {
                                // TODO open conn
                                MessageBox.Show(s.DispName);
                                s.Conn();
                            }
                        }
                        HideMe();
                        break;
                    }

                case Key.Down:
                    {
                        lock (_keyDownLocker)
                        {
                            if (_vmSearchBox.SelectedServerTextIndex < _vmSearchBox.DispServerList.Count - 1)
                            {
                                ++_vmSearchBox.SelectedServerTextIndex;
                                ListBoxSelections.ScrollIntoView(ListBoxSelections.SelectedItem);
                            }
                        }

                        break;
                    }

                case Key.Up:
                    {
                        lock (_keyDownLocker)
                        {
                            if (_vmSearchBox.SelectedServerTextIndex > 0)
                            {
                                --_vmSearchBox.SelectedServerTextIndex;
                                ListBoxSelections.ScrollIntoView(ListBoxSelections.SelectedItem);
                            }
                        }

                        break;
                    }

                case Key.PageUp:
                    {
                        lock (_keyDownLocker)
                        {
                            var i = _vmSearchBox.SelectedServerTextIndex - 5;
                            if (i < 0)
                                i = 0;
                            _vmSearchBox.SelectedServerTextIndex = i;
                            ListBoxSelections.ScrollIntoView(ListBoxSelections.SelectedItem);
                        }

                        break;
                    }

                case Key.PageDown:
                    {
                        lock (_keyDownLocker)
                        {
                            var i = _vmSearchBox.SelectedServerTextIndex + 5;
                            if (i > _vmSearchBox.DispServerList.Count - 1)
                                i = _vmSearchBox.DispServerList.Count - 1;
                            _vmSearchBox.SelectedServerTextIndex = i;
                            ListBoxSelections.ScrollIntoView(ListBoxSelections.SelectedItem);
                        }

                        break;
                    }
            }
        }
    }
}
