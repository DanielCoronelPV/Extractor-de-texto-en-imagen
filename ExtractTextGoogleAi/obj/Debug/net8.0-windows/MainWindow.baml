   M S B A M L   `   `   `  ����  ExtractTextGoogleAi$    ExtractTextGoogleAi.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$#  Extractor de texto en imágen��5   	   $	��600��6.   $	��800��6;    � Icon$ Assets/favicon.ico��5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $��* �6   �� 5
      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0,0,0,10q�6.   $��Center=�6@   .+�5      �� -   5         ��Name$ 	btnUpload��5      $��📤 Subir Imagen��6(   $	��120��6B   $��5q�6N   6Y   �� 5      -   5        $ btnCopy��5      $��📋 Copiar Texto��6&   $	��120��6@   $��5q�6L   6W   �� 5      -   5        $ btnClear��5      $��🧹 Limpiar��6'   $	��120��6<   $��5q�6H   6S   �� 5      -   5        $ 	btnExport��5      $��Exportar a CSV��6(   $	��120��6^   $��5q�6j   �� 5      -   5        $ btnEditarGuardar��5      $��✏️ Editar��6/   $	��120��6C   $��5q�6O   6Z   5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��2* �6#   �� 5      $��3* �6#   .+�5      �� 
�������$��1q�6(   $��5q�6<   ._�5      (�  (� Stretch$ Uniform=�6   .��5      � .+�5      �� -   5         ��Name$ 
imgPreview��5       �� 5       -   5         ��Name$ overlayCanvas��5    !   	 J� IsHitTestVisible$	 False��68   
������ 6Q   5!      5"      5#      &   System.Windows.Controls.DataGrid  5+      -   5        
  Name$
 dataGridResultados��5+      $��1��63      AutoGenerateColumns$ False��6C      CanUserAddRows$ False��6_   $��10q�6v      
IsReadOnly$
 True��6�      Columns 5,      0  *System.Windows.Controls.DataGridTextColumn  5-      ,  &System.Windows.Controls.DataGridColumn   Header$ Código��6)      
IsReadOnly$ False��6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *Codigo+6T     5.      $ Descripción��6)   $ False��6^    6>   �� *Descripcion+6^     5/      $ Cantidad��6)   $ False��6X    6;   �� *
Cantidad+6X     50      $ Precio Unitario��6)   $ False��6e    6B   �� *PrecioUnitario+6e   52      54      57      58      