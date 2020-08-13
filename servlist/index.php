<html>
<head>
<title>Sniper MP Server Test</title>
</head>
<body>
<table border="1">
<tr><td>
<h1>Register</h1>
<form action="regsvr.php" method="post">
Name:<input type="text" name="name"/>
Port:<input type="text" name="port"/>
<input type="hidden" name="valid" value="sniper">
<input type="submit"/>
</form>
</tr></td>
<tr><td>
<h1>Update</h1>
<form action="updsvr.php" method="post">
Name:<input type="text" name="name"/>
Port:<input type="text" name="port"/>
<input type="hidden" name="valid" value="sniper">
<input type="submit"/>
</form>
</tr></td>
<tr><td>
<h1>Remove</h1>
<form action="remsvr.php" method="post">
Name:<input type="text" name="name"/>
Port:<input type="text" name="port"/>
<input type="hidden" name="valid" value="sniper">
<input type="submit"/>
</form>
</tr></td>
</table>
</body>