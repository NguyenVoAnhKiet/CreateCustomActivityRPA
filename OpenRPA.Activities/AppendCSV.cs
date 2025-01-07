using System;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Windows.Controls;
using System.Windows;

namespace OpenRPA.Utilities
{
    // Đánh dấu lớp AppendCSV để sử dụng với AppendCSVDesigner trong trình thiết kế WF.
    [Designer(typeof(AppendCSVDesigner))]
    public class AppendCSV : CodeActivity // Kế thừa từ CodeActivity để thực thi mã tùy chỉnh.
    {
        // Constructor của lớp AppendCSV.
        public AppendCSV() : base()
        {
            // Đặt giá trị mặc định cho Delimiter là dấu phẩy (,).
            Delimiter = new InArgument<string>(",");

            // Tạo một AttributeTableBuilder để tùy chỉnh các thuộc tính.
            var builder = new AttributeTableBuilder();
            // Thêm thuộc tính Editor tùy chỉnh cho Encoding, sử dụng EncodingPropertyEditor.
            builder.AddCustomAttributes(typeof(AppendCSV), nameof(Encoding), new EditorAttribute(typeof(EncodingPropertyEditor), typeof(PropertyValueEditor)));
            // Thêm AttributeTable vào MetadataStore để đăng ký các thuộc tính tùy chỉnh.
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        // Định nghĩa thuộc tính FilePath, đường dẫn đến tệp CSV.
        [Category("Input")] // Thuộc tính thuộc nhóm "Input".
        [RequiredArgument] // Thuộc tính bắt buộc phải được cung cấp.
        [Description("Path to the CSV file.")] // Mô tả thuộc tính.
        public InArgument<string> FilePath { get; set; }

        // Định nghĩa thuộc tính DataTable, dữ liệu sẽ được ghi vào tệp CSV.
        [Category("Input")]
        [RequiredArgument]
        [Description("The DataTable to append to the CSV file.")]
        public InArgument<DataTable> DataTable { get; set; }

        // Định nghĩa thuộc tính Delimiter, ký tự phân tách giữa các giá trị trong tệp CSV.
        [Category("Input")]
        [Description("The delimiter used in the CSV file (e.g., ',', ';', '\t').")]
        public InArgument<string> Delimiter { get; set; }

        // Định nghĩa thuộc tính Encoding, quy định cách mã hóa ký tự trong tệp CSV.
        [Category("Input")]
        [Description("The encoding of the CSV file (e.g., UTF-8, ASCII).")]
        [Editor(typeof(EncodingPropertyEditor), typeof(PropertyValueEditor))] // Sử dụng EncodingPropertyEditor để chỉnh sửa thuộc tính này.
        public InArgument<Encoding> Encoding { get; set; }

        // Định nghĩa thuộc tính IncludeHeader, xác định xem có ghi header (tên cột) vào tệp CSV hay không (nếu file mới hoặc rỗng).
        [Category("Input")]
        [Description("Specifies whether to include the header row if the file is new. Default is true.")]
        public InArgument<bool> IncludeHeader { get; set; } = new InArgument<bool>(true); // Giá trị mặc định là true.

        // Phương thức Execute được gọi khi hoạt động được thực thi.
        protected override void Execute(CodeActivityContext context)
        {
            // Lấy giá trị của các input argument từ context.
            string filePath = FilePath.Get(context);
            DataTable dataTable = DataTable.Get(context);
            string delimiter = Delimiter.Get(context);
            Encoding encoding = Encoding.Get(context);
            bool includeHeader = IncludeHeader.Get(context);

            // Kiểm tra đầu vào: FilePath không được null hoặc rỗng.
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("FilePath cannot be null or empty.");
            }

            // Kiểm tra đầu vào: DataTable không được null.
            if (dataTable == null)
            {
                throw new ArgumentException("DataTable cannot be null.");
            }

            // Đặt giá trị mặc định cho delimiter nếu nó không được cung cấp.
            if (string.IsNullOrEmpty(delimiter))
            {
                delimiter = ","; // Giá trị mặc định là dấu phẩy.
            }

            // Đặt giá trị mặc định cho encoding nếu nó không được cung cấp.
            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8; // Giá trị mặc định là UTF-8.
            }

            // Kiểm tra xem file có tồn tại không.
            bool fileExists = File.Exists(filePath);
            // Kiểm tra xem file có dữ liệu không (kích thước lớn hơn 0).
            bool fileHasData = fileExists && new FileInfo(filePath).Length > 0;

            // Kiểm tra số lượng cột trước khi ghi bất cứ thứ gì vào file
            if (fileHasData)
            {
                int existingCsvColumnCount = 0;
                // Đọc dòng đầu tiên của file CSV để lấy số lượng cột hiện có
                using (StreamReader reader = new StreamReader(filePath, encoding))
                {
                    string existingHeaderLine = reader.ReadLine();
                    if (!string.IsNullOrEmpty(existingHeaderLine))
                    {
                        existingCsvColumnCount = existingHeaderLine.Split(delimiter.ToCharArray()).Length;
                    }
                }

                // Nếu số lượng cột trong DataTable không khớp với file CSV hiện có, ném ra ngoại lệ
                if (existingCsvColumnCount != dataTable.Columns.Count)
                {
                    throw new ArgumentException("The number of columns in the DataTable does not match the existing CSV file.");
                }
            }
            else
            {
                // Nếu file mới hoặc trống, coi như nó có 0 cột để kiểm tra header
                // Nếu DataTable không có cột nào và IncludeHeader là true, ném ra ngoại lệ
                if (dataTable.Columns.Count == 0 && includeHeader)
                {
                    throw new ArgumentException("Cannot write header to a new file when DataTable has no columns.");
                }
            }

            // Ghi/Append vào tệp CSV.
            using (StreamWriter writer = new StreamWriter(filePath, fileHasData, encoding))
            {
                // Ghi header nếu file mới hoặc rỗng và IncludeHeader là true.
                if (!fileHasData && includeHeader)
                {
                    // Tạo dòng header từ tên cột của DataTable, escape các giá trị nếu cần.
                    string headerLine = string.Join(delimiter, dataTable.Columns.Cast<DataColumn>().Select(column => EscapeValue(column.ColumnName, delimiter)));
                    writer.WriteLine(headerLine);
                }

                // Ghi các dòng dữ liệu từ DataTable vào file CSV.
                foreach (DataRow row in dataTable.Rows)
                {
                    // Tạo dòng dữ liệu từ các giá trị của DataRow, escape các giá trị nếu cần.
                    string dataLine = string.Join(delimiter, row.ItemArray.Select(item => EscapeValue(item?.ToString(), delimiter)));
                    writer.WriteLine(dataLine);
                }
            }
        }

        // Hàm EscapeValue để xử lý các giá trị có chứa ký tự đặc biệt (dấu phân cách, dấu nháy kép, xuống dòng).
        private string EscapeValue(string value, string delimiter)
        {
            // Nếu giá trị là null hoặc rỗng, trả về chuỗi rỗng.
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Kiểm tra xem giá trị có cần escape hay không (chứa delimiter, dấu nháy kép hoặc xuống dòng).
            bool needsEscaping = value.Contains(delimiter) || value.Contains("\"") || value.Contains("\n");

            // Nếu cần escape, đặt giá trị trong dấu nháy kép và thay thế dấu nháy kép bằng hai dấu nháy kép.
            if (needsEscaping)
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            // Nếu không cần escape, trả về giá trị nguyên bản.
            return value;
        }
    }

    // Lớp EncodingPropertyEditor tùy chỉnh trình chỉnh sửa cho thuộc tính Encoding.
    public class EncodingPropertyEditor : PropertyValueEditor
    {
        // Constructor của EncodingPropertyEditor.
        public EncodingPropertyEditor()
        {
            // Tạo DataTemplate cho inline editor trong constructor.
            this.InlineEditorTemplate = CreateInlineEditorTemplate();
        }

        // Phương thức hỗ trợ tạo DataTemplate cho inline editor.
        private DataTemplate CreateInlineEditorTemplate()
        {
            DataTemplate inlineEditorTemplate = new DataTemplate();
            // Tạo một StackPanel để chứa ComboBox.
            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            // Tạo một ComboBox để chọn encoding.
            FrameworkElementFactory encoding = new FrameworkElementFactory(typeof(System.Windows.Controls.ComboBox));
            // Đặt ItemsSource của ComboBox thành danh sách tên các encoding.
            encoding.SetValue(System.Windows.Controls.ComboBox.ItemsSourceProperty, Encoding.GetEncodings().Select(x => x.Name));
            // Binding SelectedValue của ComboBox với thuộc tính Value, sử dụng EncodingNameConverter.
            encoding.SetValue(System.Windows.Controls.ComboBox.SelectedValueProperty, new System.Windows.Data.Binding("Value") { Mode = System.Windows.Data.BindingMode.TwoWay, Converter = new EncodingNameConverter() });
            // Đặt margin cho ComboBox.
            encoding.SetValue(System.Windows.Controls.ComboBox.MarginProperty, new Thickness(2, 0, 0, 0));
            // Thêm ComboBox vào StackPanel.
            stack.AppendChild(encoding);

            // Đặt VisualTree của DataTemplate thành StackPanel.
            inlineEditorTemplate.VisualTree = stack;
            return inlineEditorTemplate;
        }

        // Thuộc tính EditMode chỉ định rằng đây là một inlined editor (không phải dialog).
        public new static Type EditMode { get; } = typeof(ExtendedPropertyValueEditor);
    }

    // Lớp EncodingNameConverter chuyển đổi giữa InArgument<Encoding> và tên encoding (string).
    public class EncodingNameConverter : System.Windows.Data.IValueConverter
    {
        // Phương thức Convert chuyển đổi từ InArgument<Encoding> sang string (tên encoding hoặc biểu thức).
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var encoding = value as System.Activities.InArgument<System.Text.Encoding>;

            // Nếu giá trị là InArgument<Encoding> và Expression không null.
            if (encoding != null && encoding.Expression != null)
            {
                // Nếu Expression là Literal<Encoding>, trả về tên WebName của encoding.
                if (encoding.Expression is System.Activities.Expressions.Literal<System.Text.Encoding> literal)
                {
                    return literal.Value.WebName;
                }
                // Nếu Expression là VariableValue<Encoding>, trả về tên biến.
                else if (encoding.Expression is System.Activities.Expressions.VariableValue<System.Text.Encoding> variableValue)
                {
                    return variableValue.Variable.Name;
                }
                // Nếu là loại Expression khác, trả về chuỗi biểu diễn của Expression.
                else
                {
                    return encoding.Expression.ToString();
                }
            }

            // Nếu không phải InArgument<Encoding> hoặc Expression là null, trả về giá trị gốc.
            return value;
        }

        // Phương thức ConvertBack chuyển đổi từ string (tên encoding) sang InArgument<Encoding>.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string encodingName)
            {
                // Nếu giá trị là chuỗi rỗng hoặc null, trả về null.
                if (string.IsNullOrEmpty(encodingName))
                {
                    return null;
                }
                try
                {
                    // Thử lấy đối tượng Encoding từ tên encoding và trả về InArgument<Encoding> tương ứng.
                    return new InArgument<Encoding>(Encoding.GetEncoding(encodingName));
                }
                catch (ArgumentException)
                {
                    // Xử lý trường hợp tên encoding không hợp lệ.
                    // Option 1: Ném ra ngoại lệ (Khuyến nghị cho hầu hết các trường hợp).
                    throw new ArgumentException($"Invalid encoding name: '{encodingName}'");

                    // Option 2: Trả về giá trị mặc định (ví dụ: UTF-8) và ghi log lỗi.
                    // System.Diagnostics.Debug.WriteLine($"Invalid encoding name: '{encodingName}'. Using UTF-8 as default.");
                    // return new InArgument<Encoding>(Encoding.UTF8);
                }
            }
            // Nếu giá trị không phải là string, trả về giá trị gốc.
            return value;
        }
    }
}