<?php
mysqli_query($sql,"DELETE FROM servers WHERE last < (NOW() + INTERVAL 6 HOUR)");
$data = mysqli_query($sql,"SELECT addr, name, last FROM servers");
$file = fopen("list.txt","w");
while($row = mysqli_fetch_array($data))
	fwrite($file,$row[0].";".$row[1]." - ".$row[2]."\n");
fclose($file);
?>