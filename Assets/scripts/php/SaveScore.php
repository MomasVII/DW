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

    require_once('mysqli_db.php');
    $mysqli_db = new mysqli_db($port = null); //can also specify port if needed

    if(isset($_POST["namePost"])) {
        $name = $_POST["namePost"];
    } else {
        echo "N not set";
    }

    if(isset($_POST["scorePost"])) {
        $score = $_POST["scorePost"];
    } else {
        echo "S not set";
    }
    if(isset($_POST["carusedPost"])) {
        $carused = $_POST["carusedPost"];
    } else {
        echo "CU not set";
    }

    if(isset($_POST["level"])) {
        $level = $_POST["level"];
    } else {
        echo "L not set";
    }

    $insert_data = array(
        'User' => $name,
        'CarUsed' => $carused,
        'Score' => $score,
        'Level' => $level
    );


    $insert_result = $mysqli_db->insert('Highscores', $insert_data);

    if($insert_result){
        echo "success";
        var_dump($insert_data);
    }else{
        echo "fail";
        var_dump($insert_data);
    }

?>
