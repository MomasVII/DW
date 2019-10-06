<?php

    error_reporting(E_ALL);
    ini_set('display_errors', 1);
    error_reporting(2047);

    //database host (usually localhost)
    define('DB_HOST', 'localhost');

    //database username
    define('DB_USER', 'MomasVII');

    //database password
    define('DB_PASS', 'd4HTsv6!-5~`');

    //database schema name
    define('DB_NAME', 'DriftWorlds');

    $level = '';
    if(isset($_GET["level"])) {
        $level = $_GET["level"];
    } else {
        echo "Level not set";
    }

    /*$rank = '';
    if(isset($_GET["rank"])) {
        $rank = $_GET["rank"];
    } else {
        echo "Level not set";
    }*/

    //instantiate mysqli_db and connect to database
    require_once('mysqli_db.php');
    $mysqli_db = new mysqli_db($port = null); //can also specify port if needed

    //this will let you run any simple query, for the times when the quick functions above restrict you too much.
    $generic_result = $mysqli_db->query('SELECT * FROM `Highscores` WHERE `Level` = "'.$level.'" ORDER by `Score` ASC LIMIT 3');

    if(!empty($generic_result)){
        foreach($generic_result as $row){
            echo $row['User'] . ",". $row['CarUsed'] . ",". $row['Score'] . ",";
        }
    }else{
        //$print .= '<p>Generic query failed.</p>';
    }























    /*$servername = "localhost";
    $server_username =  "MomasVII";
    $server_password = "d4HTsv6!-5~`";
    $dbName = "DriftWorlds";

    //make connection
    $conn = new mysqli($servername, $server_username, $server_password, $dbName);
    if(!conn){
        die("Connection failed: " . mysqli_connect_error());
    }

    $query = "SELECT * FROM `Highscores` ORDER by `Score` ASC LIMIT 3";
    $result = mysqli_query($conn, $query);

    if(mysqli_num_rows($result) > 0){
        //show data for each row
        while($row = mysqli_fetch_assoc($result)){
            echo $row['User'] . ",". $row['CarUsed'] . ",". $row['Score'] . ",";
        }
    }*/
?>
