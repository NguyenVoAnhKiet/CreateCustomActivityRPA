   M S B A M L   `   `   `  ����e  aSystem.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35wTclr-namespace:System.Activities.Presentation;assembly=System.Activities.PresentationSystem.Activities.Presentation  5      �Yclr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation#System.Activities.Presentation.View  5      �_clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation)System.Activities.Presentation.Converters  5      M IOpenRPA.Activities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:OpenRPA.UtilitiesOpenRPA.Utilities 5      )   #OpenRPA.Utilities.AppendCSVDesigner   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      dsapTclr-namespace:System.Activities.Presentation;assembly=System.Activities.Presentation     5      jsapvYclr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation     5      psapc_clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation     5      1localclr-namespace:OpenRPA.Utilities     5        � 	Resources  5	      M   GSystem.Activities.Presentation.Converters.ArgumentToExpressionConverter !  ArgumentToExpressionConverter�� 5
   
   %   &	          5      61   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6   $��
File Path:��6-   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5-      ;   5System.Activities.Presentation.View.ExpressionTextBox  5   
   4s&clr-namespace:System;assembly=mscorlib     5-      $��0��5      $��1��5          ExpressionType" ��5"         UseLocationExpression .  $��2q�5%         AcceptsReturn .    
AcceptsTab .    MinLines$ 1��5(         MaxLines$ 1��5)      "	   VerticalScrollBarVisibility$
	 Auto=�5*      $
   HorizontalScrollBarVisibility$

 Auto=�5+       � ToolTip$* $Enter the full path to the CSV file.��5,         
Expression 5!      ��  �� Path$ ModelItem.FilePath� �� Mode$ TwoWay=� �� 	Converter# [   �� ConverterParameter$ In��   OwnerActivity 5#      �� $ 	ModelItem�5-      �� 5/   
   -   5        $��0��5/      $��2��6   $	��...��6.   $��2,2,0,2q�6W   $
��Left=�6h   $��12��6�   �� 51   
   $��1��6   $��0��6   $��Data Table:��6-   R NSystem.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>.clr-namespace:System.Data;assembly=System.DataSystem.Data 5B        52   
   <d.clr-namespace:System.Data;assembly=System.Data     5B      $��1��53      $��1��54      $��2��55        System.Data.DataTable"  57       .  $��2q�5:       .  . $ 1��5=      $ 1��5>      $
	 Auto=�5?      $

 Auto=�5@      $% Select the DataTable to append.��5A       56      �� $ ModelItem.DataTable�$ TwoWay=�# [  $ In�� 58      �� $ 	ModelItem�5B      �� 5E   
   $��2��6   $��0��6   $��
Delimiter:��6-   1&clr-namespace:System;assembly=mscorlibSystem 5V        5F   
   4s&clr-namespace:System;assembly=mscorlib     5V      $��2��5G      $��1��5H      $��2��5I      " ��5K       .  $��2q�5N       .  . $ 1��5Q      $ 1��5R      $
	 Auto=�5S      $

 Auto=�5T      $- 'Enter the delimiter (default is comma).��5U       5J      �� $ ModelItem.Delimiter�$ TwoWay=�# [  $ In�� 5L      �� $ 	ModelItem�5V      �� 5Y   
   $��3��6   $��0��6   $��	Encoding:��6-   ;+clr-namespace:System.Text;assembly=mscorlibSystem.Text 5j        5Z   
   9s+clr-namespace:System.Text;assembly=mscorlib     5j      $��3��5[      $��1��5\      $��2��5]        System.Text.Encoding"  5_       .  $��2q�5b       .  . $ 1��5e      $ 1��5f      $
	 Auto=�5g      $

 Auto=�5h      $1 +Enter the file encoding (default is UTF-8).��5i       5^      �� $ ModelItem.Encoding�$ TwoWay=�# [  $ In�� 5`      �� $ 	ModelItem�5j      �� 5m   
   $��4��6   $��1��6    $��2��60   $��Include Header��6D   $��2q�6�    d� 	IsChecked 6]   �� $. (ModelItem.IncludeHeader.Expression.Value�$ TwoWay=�6�   5n      5o      