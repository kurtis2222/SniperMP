<?php
include "sqlsvr.php";
if($_POST && $_POST["valid"] == "sniper")
{
if(!is_numeric($_POST["port"])) return;
$_POST["name"] = str_replace("\"","\\\"",$_POST["name"]);
$sql = mysqli_connect($SQL_HOST,$SQL_USER,$SQL_PASS,$SQL_BASE);
mysqli_query($sql,"UPDATE servers SET name=LEFT(\"".$_POST["name"]."\",24),last=(CURRENT_TIMESTAMP + INTERVAL 7 HOUR) WHERE addr=\"".$_SERVER["REMOTE_ADDR"].":".$_POST["port"]."\";");
include "lstsvr.php";
mysqli_close($sql);
}
?>