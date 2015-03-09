The home page for this library can be found at http://www.csvreader.com. For
questions or comments, please email bruce@csvreader.com.


How to use the library inside Visual Studio .Net:

VB.Net:

1. After creating the project that you want to use the parser in, find the
Solution Explorer window. This could be located in different spots depending on
your preferences, but clicking on View->Server Explorer should pop it up from
wherever it may be hiding.

2. The top level in your Solution Explorer will be the solution. Under that
will be the project you’re working on. Under the project, there will be an
expandable item named References. Expand that out. Now right click on the word
References, and choose Add Reference.

3. You should now be in a new window, with the .Net tab selected for you. Click
on the button at the top right that says Browse. Use the file open window that
pops up to navigate to the folder on your system where you’ve extracted DataStreams.dll
to from the downloaded zip file. Click on the open button. DataStreams.dll should
now be shown in the Selected Components section of the window at the bottom of
the form. Click on the OK button at the bottom right and you should be back to
your Solution Explorer window with DataStreams now listed under References.

4. At this point, you can use the CsvReader class from your VB.Net code, but
you’ll have to fully qualify it as in 

	Dim reader As New DataStreams.Csv.CsvReader("C:\test.csv")

To get rid of this annoyance, you will need to go back to your Solution
Explorer window, right click on the project, and go to Properties. On the left
side of the window that will pop up, you will need to go to
Common Properties->Imports. On the right side now there should be a box labeled
Namespace:. Type DataStreams.Csv into that box and click on the Add Import button. Now
click on the OK button to get back to your code. You should now be able to
change the line of code above to

	Dim reader As New CsvReader("C:\test.csv")




C#:

1. After creating the project that you want to use the parser in, find the
Solution Explorer window. This could be located in different spots depending
on your preferences, but clicking on View->Server Explorer should pop it up
from wherever it may be hiding.

2. The top level in your Solution Explorer will be the solution. Under that
will be the project you’re working on. Under the project, there will be an
expandable item named References. Expand that out. Now right click on the word
References, and choose Add Reference.

3. You should now be in a new window, with the .Net tab selected for you. Click
on the button at the top right that says Browse. Use the file open window that
pops up to navigate to the folder on your system where you’ve extracted DataStreams.dll
to from the downloaded zip file. Click on the open button. DataStreams.dll should
now be shown in the Selected Components section of the window at the bottom of
the form. Click on the OK button at the bottom right and you should be back to
your Solution Explorer window with DataStreams now listed under References.

4. At the top of the C# code file you’re working on, add the following line

	using DataStreams.Csv;

You should now be able to use the code examples from the site,
http://www.csvreader.com/csv_samples.php, as is in your file.

