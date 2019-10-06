<?php
    $servername = "localhost";
    $server_username =  "MomasVII";
    $server_password = "d4HTsv6!-5~`";
    $dbName = "DriftWorlds";

    //make connection
    $conn = new mysqli($servername, $server_username, $server_password, $dbName);
    if(!conn){
        die("Connection failed: " . mysqli_connect_error());
    }

    $query = "DELETE FROM `Highscores`";

    if (mysqli_query($conn, $query)) {
        echo "Record deleted successfully";
    } else {
        echo "Error deleting record: " . mysqli_error($conn);
    }

    $sql = "INSERT INTO Highscores (User, CarUsed, Score, Level) VALUES ('Reset User 1','2','99.999','Grass'), ('Reset User 2','3','98.999','Grass'), ('Reset User 3','4','97.999','Grass'), ('Reset User 1','2','99.999','Grass2'), ('Reset User 2','3','98.999','Grass2'), ('Reset User 3','4','97.999','Grass2'), ('Reset User 1','2','99.999','Lava2'), ('Reset User 2','3','98.999','Lava2'), ('Reset User 3','4','97.999','Lava2'), ('Reset User 1','2','99.999','Lava3'), ('Reset User 2','3','98.999','Lava3'), ('Reset User 3','4','97.999','Lava3'), ('Reset User 1','2','99.999','Snow'), ('Reset User 2','3','98.999','Snow'), ('Reset User 3','4','97.999','Snow')";

    if (mysqli_query($conn, $sql)) {
       echo "New record created successfully";
    } else {
       echo "Error: " . $sql . "" . mysqli_error($conn);
    }
?>
