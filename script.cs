using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace ForgeCalc
{
    public partial class MainWindow : Window
    {
        private List<string> calculationHistory = new List<string>();
        private bool isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Main Grid
            Grid mainGrid = new Grid();
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(245, 247, 250));

            // Define columns: Left Panel (300), Center (1*), Right Panel (350)
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(350) });

            // Left Panel - Module Selector
            Border leftPanel = CreateLeftPanel();
            Grid.SetColumn(leftPanel, 0);
            mainGrid.Children.Add(leftPanel);

            // Center Panel - Operations
            Border centerPanel = CreateCenterPanel();
            Grid.SetColumn(centerPanel, 1);
            mainGrid.Children.Add(centerPanel);

            // Right Panel - Results & History
            Border rightPanel = CreateRightPanel();
            Grid.SetColumn(rightPanel, 2);
            mainGrid.Children.Add(rightPanel);

            this.Content = mainGrid;
        }

        private Border CreateLeftPanel()
        {
            Border panel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 235, 240)),
                BorderThickness = new Thickness(0, 0, 1, 0),
                Padding = new Thickness(20)
            };

            StackPanel stack = new StackPanel();

            // Title
            TextBlock title = new TextBlock
            {
                Text = "ForgeCalc",
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 60, 114)),
                Margin = new Thickness(0, 0, 0, 30)
            };
            stack.Children.Add(title);

            // Module Buttons
            string[] modules = { "Algebra", "Matrix", "Geometry", "Statistics", "Graphing", "Scientific" };
            foreach (var module in modules)
            {
                Button btn = CreateModuleButton(module);
                stack.Children.Add(btn);
            }

            // Theme Toggle
            Button themeBtn = new Button
            {
                Content = "🌙 Dark Mode",
                Height = 40,
                Margin = new Thickness(0, 30, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            themeBtn.Click += ToggleTheme;
            stack.Children.Add(themeBtn);

            panel.Child = stack;
            return panel;
        }

        private Button CreateModuleButton(string text)
        {
            Button btn = new Button
            {
                Content = text,
                Height = 50,
                Margin = new Thickness(0, 0, 0, 10),
                Background = new SolidColorBrush(Color.FromRgb(59, 130, 246)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontSize = 16,
                FontWeight = FontWeights.Medium,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btn.MouseEnter += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                DoubleAnimation anim = new DoubleAnimation(1, 1.02, TimeSpan.FromMilliseconds(150));
                ScaleTransform scale = new ScaleTransform(1, 1);
                btn.RenderTransform = scale;
                btn.RenderTransformOrigin = new Point(0.5, 0.5);
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                DoubleAnimation anim = new DoubleAnimation(1.02, 1, TimeSpan.FromMilliseconds(150));
                if (btn.RenderTransform is ScaleTransform scale)
                {
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                }
            };

            btn.Click += (s, e) => LoadModule(text);
            return btn;
        }

        private Border CreateCenterPanel()
        {
            Border panel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Margin = new Thickness(10),
                Padding = new Thickness(30),
                CornerRadius = new CornerRadius(12)
            };

            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Name = "centerContent"
            };

            StackPanel stack = new StackPanel { Name = "moduleContent" };
            
            // Default: Algebra Module
            stack.Children.Add(CreateAlgebraModule());

            scrollViewer.Content = stack;
            panel.Child = scrollViewer;
            return panel;
        }

        private Border CreateRightPanel()
        {
            Border panel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 235, 240)),
                BorderThickness = new Thickness(1, 0, 0, 0),
                Padding = new Thickness(20)
            };

            StackPanel stack = new StackPanel();

            TextBlock title = new TextBlock
            {
                Text = "Results & History",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 60, 114))
            };
            stack.Children.Add(title);

            // Results Display
            Border resultsBox = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                Padding = new Thickness(15),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 20),
                MinHeight = 150
            };

            TextBlock resultsText = new TextBlock
            {
                Name = "resultsDisplay",
                Text = "Results will appear here...",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105))
            };
            resultsBox.Child = resultsText;
            stack.Children.Add(resultsBox);

            // History
            TextBlock historyTitle = new TextBlock
            {
                Text = "Calculation History",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 60, 114))
            };
            stack.Children.Add(historyTitle);

            ScrollViewer historyScroll = new ScrollViewer
            {
                MaxHeight = 300,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            StackPanel historyStack = new StackPanel { Name = "historyPanel" };
            historyScroll.Content = historyStack;
            stack.Children.Add(historyScroll);

            // Export Button
            Button exportBtn = new Button
            {
                Content = "📊 Export History",
                Height = 40,
                Margin = new Thickness(0, 20, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            exportBtn.Click += ExportHistory;
            stack.Children.Add(exportBtn);

            panel.Child = stack;
            return panel;
        }

        private UIElement CreateAlgebraModule()
        {
            StackPanel module = new StackPanel();

            TextBlock header = new TextBlock
            {
                Text = "Algebra Module",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 60, 114))
            };
            module.Children.Add(header);

            // Linear Equation
            GroupBox linearGroup = new GroupBox
            {
                Header = "Linear Equation (ax + b = 0)",
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(15)
            };

            StackPanel linearStack = new StackPanel();
            TextBox aBox = CreateInputField("Coefficient a:");
            TextBox bBox = CreateInputField("Coefficient b:");
            Button solveLinear = CreateActionButton("Solve Linear");
            solveLinear.Click += (s, e) => SolveLinear(aBox.Text, bBox.Text);

            linearStack.Children.Add(aBox);
            linearStack.Children.Add(bBox);
            linearStack.Children.Add(solveLinear);
            linearGroup.Content = linearStack;
            module.Children.Add(linearGroup);

            // Quadratic Equation
            GroupBox quadGroup = new GroupBox
            {
                Header = "Quadratic Equation (ax² + bx + c = 0)",
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(15)
            };

            StackPanel quadStack = new StackPanel();
            TextBox qaBox = CreateInputField("Coefficient a:");
            TextBox qbBox = CreateInputField("Coefficient b:");
            TextBox qcBox = CreateInputField("Coefficient c:");
            Button solveQuad = CreateActionButton("Solve Quadratic");
            solveQuad.Click += (s, e) => SolveQuadratic(qaBox.Text, qbBox.Text, qcBox.Text);

            quadStack.Children.Add(qaBox);
            quadStack.Children.Add(qbBox);
            quadStack.Children.Add(qcBox);
            quadStack.Children.Add(solveQuad);
            quadGroup.Content = quadStack;
            module.Children.Add(quadGroup);

            return module;
        }

        private TextBox CreateInputField(string label)
        {
            StackPanel container = new StackPanel { Margin = new Thickness(0, 0, 0, 15) };
            
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85))
            };

            TextBox box = new TextBox
            {
                Height = 35,
                Padding = new Thickness(10),
                FontSize = 14,
                BorderBrush = new SolidColorBrush(Color.FromRgb(203, 213, 225)),
                BorderThickness = new Thickness(1)
            };

            container.Children.Add(lbl);
            container.Children.Add(box);

            var parent = FindParent();
            if (parent != null) parent.Children.Add(container);

            return box;
        }

        private StackPanel FindParent()
        {
            if (this.Content is Grid mainGrid)
            {
                foreach (var child in mainGrid.Children)
                {
                    if (child is Border border && Grid.GetColumn(border) == 1)
                    {
                        if (border.Child is ScrollViewer sv && sv.Content is StackPanel sp)
                        {
                            return sp.Children[0] as StackPanel;
                        }
                    }
                }
            }
            return null;
        }

        private Button CreateActionButton(string text)
        {
            Button btn = new Button
            {
                Content = text,
                Height = 45,
                Margin = new Thickness(0, 10, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(139, 92, 246)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontSize = 15,
                FontWeight = FontWeights.Medium,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btn.MouseEnter += (s, e) => btn.Background = new SolidColorBrush(Color.FromRgb(124, 58, 237));
            btn.MouseLeave += (s, e) => btn.Background = new SolidColorBrush(Color.FromRgb(139, 92, 246));

            return btn;
        }

        private void SolveLinear(string aStr, string bStr)
        {
            try
            {
                double a = double.Parse(aStr);
                double b = double.Parse(bStr);

                if (Math.Abs(a) < 0.0001)
                {
                    DisplayResult("Error: Coefficient 'a' cannot be zero.");
                    return;
                }

                double x = -b / a;
                string result = $"Linear Equation Solution:\n\n" +
                               $"{a}x + {b} = 0\n\n" +
                               $"Step 1: {a}x = -{b}\n" +
                               $"Step 2: x = -{b}/{a}\n\n" +
                               $"Solution: x = {x:F4}";

                DisplayResult(result);
                AddToHistory($"Linear: {a}x + {b} = 0 → x = {x:F4}");
            }
            catch
            {
                DisplayResult("Error: Invalid input values.");
            }
        }

        private void SolveQuadratic(string aStr, string bStr, string cStr)
        {
            try
            {
                double a = double.Parse(aStr);
                double b = double.Parse(bStr);
                double c = double.Parse(cStr);

                if (Math.Abs(a) < 0.0001)
                {
                    DisplayResult("Error: Coefficient 'a' cannot be zero.");
                    return;
                }

                double discriminant = b * b - 4 * a * c;
                StringBuilder result = new StringBuilder();
                result.AppendLine("Quadratic Equation Solution:\n");
                result.AppendLine($"{a}x² + {b}x + {c} = 0\n");
                result.AppendLine($"Discriminant: Δ = b² - 4ac = {discriminant:F4}\n");

                if (discriminant > 0)
                {
                    double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                    double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                    result.AppendLine("Two real solutions:");
                    result.AppendLine($"x₁ = {x1:F4}");
                    result.AppendLine($"x₂ = {x2:F4}");
                    AddToHistory($"Quadratic: {a}x² + {b}x + {c} = 0 → x₁={x1:F4}, x₂={x2:F4}");
                }
                else if (Math.Abs(discriminant) < 0.0001)
                {
                    double x = -b / (2 * a);
                    result.AppendLine("One real solution:");
                    result.AppendLine($"x = {x:F4}");
                    AddToHistory($"Quadratic: {a}x² + {b}x + {c} = 0 → x={x:F4}");
                }
                else
                {
                    double realPart = -b / (2 * a);
                    double imagPart = Math.Sqrt(-discriminant) / (2 * a);
                    result.AppendLine("Two complex solutions:");
                    result.AppendLine($"x₁ = {realPart:F4} + {imagPart:F4}i");
                    result.AppendLine($"x₂ = {realPart:F4} - {imagPart:F4}i");
                    AddToHistory($"Quadratic: Complex solutions");
                }

                DisplayResult(result.ToString());
            }
            catch
            {
                DisplayResult("Error: Invalid input values.");
            }
        }

        private void DisplayResult(string text)
        {
            if (this.Content is Grid mainGrid)
            {
                foreach (var child in mainGrid.Children)
                {
                    if (child is Border border && Grid.GetColumn(border) == 2)
                    {
                        if (border.Child is StackPanel sp)
                        {
                            foreach (var item in sp.Children)
                            {
                                if (item is Border rb && rb.Child is TextBlock tb)
                                {
                                    tb.Text = text;
                                    tb.Foreground = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                                    tb.FontWeight = FontWeights.Normal;

                                    // Animate
                                    DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                                    rb.BeginAnimation(OpacityProperty, fadeIn);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddToHistory(string entry)
        {
            calculationHistory.Insert(0, $"{DateTime.Now:HH:mm:ss} - {entry}");
            
            if (this.Content is Grid mainGrid)
            {
                foreach (var child in mainGrid.Children)
                {
                    if (child is Border border && Grid.GetColumn(border) == 2)
                    {
                        if (border.Child is StackPanel sp)
                        {
                            foreach (var item in sp.Children)
                            {
                                if (item is ScrollViewer sv && sv.Content is StackPanel histStack)
                                {
                                    histStack.Children.Clear();
                                    foreach (var hist in calculationHistory.Take(20))
                                    {
                                        TextBlock histItem = new TextBlock
                                        {
                                            Text = hist,
                                            FontSize = 12,
                                            Margin = new Thickness(0, 0, 0, 8),
                                            TextWrapping = TextWrapping.Wrap,
                                            Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139))
                                        };
                                        histStack.Children.Add(histItem);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadModule(string moduleName)
        {
            DisplayResult($"{moduleName} module loaded.\n\nUse the controls to perform calculations.");
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            Button btn = sender as Button;
            btn.Content = isDarkTheme ? "☀️ Light Mode" : "🌙 Dark Mode";
            DisplayResult($"Theme changed to {(isDarkTheme ? "Dark" : "Light")} mode.");
        }

        private void ExportHistory(object sender, RoutedEventArgs e)
        {
            if (calculationHistory.Count == 0)
            {
                DisplayResult("No history to export.");
                return;
            }

            string export = string.Join("\n", calculationHistory);
            Clipboard.SetText(export);
            DisplayResult($"History exported to clipboard!\n\n{calculationHistory.Count} entries copied.");
        }
    }
}
