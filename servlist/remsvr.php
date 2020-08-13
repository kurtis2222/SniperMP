<?php
include "sqlsvr.php";
if($_POST && $_POST["valid"] == "sniper")
{
if(!is_numeric($_POST["port"])) return;
$sql = mysqli_connect($SQL_HOST,$SQL_USER,$SQL_PASS,$SQL_BASE);
echo "DELETE FROM servers WHERE addr=\"".$_SERVER["REMOTE_ADDR"].":".$_POST["port"]."\";";
mysqli_query($sql,"DELETE FROM servers WHERE addr=\"".$_SERVER["REMOTE_ADDR"].":".$_POST["port"]."\";");
include "lstsvr.php";
mysqli_close($sql);
}
?>