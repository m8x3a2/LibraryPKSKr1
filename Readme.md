Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.3
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.3

Add-Migration InitialCreate
Update-Database

![Картинка](Image/im1.png)
![Картинка](Image/im2.png)
![Картинка](Image/im3.png)
![Картинка](Image/im4.png)

проверка на то что нет таких же жанров, а также валидация ISBN по формату ввода

