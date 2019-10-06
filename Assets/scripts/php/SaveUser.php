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


    $name = $_POST["username"];

    $insert_data = array(
        'Username' => $name,
        'Created' => date("Y-m-d")
    );

    //this will insert the above data into the table using the keys as the column names and the values as the data.
    $secretKey="insertsecretcodehere";
    //$real_hash = md5($name);

    //if($real_hash == $hash){
        $insert_result = $mysqli_db->insert('Users', $insert_data);
        if($insert_result){
            $print .= '<p>Insert query succeeded.</p>';
        }else{
            $print .= '<p>Insert query failed.</p>';
        }
    //}

?>
