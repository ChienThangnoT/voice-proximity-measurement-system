   M S B A M L   `   `   `  ����T  PVoiceProximityMeasurement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:VoiceProximityMeasurementVoiceProximityMeasurement  5   	   *    $VoiceProximityMeasurement.MainWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:VoiceProximityMeasurement     5   	     � Title$  Speech To Text Sample��5   	   $	��450��6'   $	��800��64   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   .+�5   
   �� -   5         ��Name$ ImageListView��5       $��0��6   $��	10,20,0,0q�68   $��Center=�6K   $��Center=�6h   ��5      �� *Images+��5      �� 5      .R�5      �  � Orientation$ 
Horizontal=�6    5      ��5      �� 5      .R�5      �� $��5q�6r   ��6   �� *Bitmap+��66   �� *
ImageWidth+��6S   �� *ImageHeight+6r   5      5      �� 5    
   -   5         ��Name$ CountdownText��5    !   $��0��6   $��	10,79,0,0q�68   $��Time out: 00:00��6K   $
��Left=�6b   $	��Top=�6}   �� 5!   
   -   5        $ RemainingIterationsText��5!      $��
10,100,0,0q�65   $+��%Number of remaining measurements:   8��6I   $
��Left=�6v   $	��Top=�6�   $
��Wrap=�6�   $	��104��6�   �� 5"   
   $��0��6   $��
10,200,0,0q�6!   $
��Wrap=�65   Intermediate Result6^   �� 5#   
   $��1��6   $��
10,0,10,10q�6   $��1q�63   
�������._�5$      �� ��6   �� *Speech+64   5%      �� 5'   
   $��2��6   $��	10,10,0,0q�6!   $
��Wrap=�64   Final Result6V   �� 5(   
   $��3��6   $��
10,0,10,10q�6   $��1q�63   
�������._�5)      �� ���	#FFEDEDF4$��14(�63   $
��Wrap=�6A   ��6V   �� *Transcribed+6u   5*      ]� 5+   
   $��4��6   $��	10,0,10,0q�6"   .+�5,      �� Status6    �� 5-      $��10,0,0,0q�6   ���	#FFFCFFE9��6(   �� *Status+6Y   5.      �� 5/   
   $��5��6   $��0,10,0,0q�6"    �� Orientation$ 
Horizontal=�64   $��Center=�6M   .+�50      �� -   5        
�������$��	0,0,20,15q�50   )   $	��100��6<   ��6]   �� *	StartStop+ �� Mode$ TwoWay=�6�   �� 51      -   5        
�������$��	0,0,20,15q�51   )   $	��100��6<   $��
Clear Text��6H   6q   �� 52      -   5        
�������$��	0,0,20,15q�52   )   $	��100��6<   $��Generate Chart��6H   6   �� 54      -   5         ��Name$ ResultButton��54   }   
�������$��	0,0,20,15q�6(   $	��150��6;   $��Result��6G   ${�False��6X   6}   56      57      58      