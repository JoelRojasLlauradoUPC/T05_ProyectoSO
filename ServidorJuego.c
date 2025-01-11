#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

#define PORT 50071 // port from which we will listen
#define MAX_CLIENTS 100 // maximum nuber of pending petitions


struct thread_data {
	int sock_conn;
	int IdentificadorPartida;
	int IdentificadorJugadorPartida;
    int enPartida;
	MYSQL* conn;
	char username[20];
};

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
char connected_players[MAX_CLIENTS][20];
int player_count = 0;
int client_count = 0;
struct thread_data* sockets[MAX_CLIENTS];

//INTERNAL FUNCTIONS
MYSQL* init_mysql_connection();
void* handle_client(void* data);
void process_request(char* request, char* response, MYSQL* conn, char* username, int sender_sock);
void add_player(char* username);
void remove_player(char* username);
void get_connected_players(char* response);

//INTERACTION
void register_user(char* username, char* password, char* response, MYSQL* conn);
void login_user(char* username, char* password, char* response, MYSQL* conn);
void get_players_played_with(char* username, char* response, MYSQL* conn);

//FUNCTION TO ADD PLAYER TO THE CONNECTED LIST
// |-----------------------------------------------------------------------------------------------------------|
// | Function: add_player                                                                                      |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Adds a player to the list of connected players, ensuring thread safety using a mutex.        |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* username: The username of the player to add.                                                     |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void add_player(char* username)
{
	pthread_mutex_lock(&mutex);

	if (player_count < MAX_CLIENTS)
    {
		strcpy(connected_players[player_count], username);
		player_count++;
	}

	pthread_mutex_unlock(&mutex);
}

//FUNCTION TO REMOVE PLAYER FROM THE CONNECTED LIST
// |-----------------------------------------------------------------------------------------------------------|
// | Function: remove_player                                                                                   |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Removes a player from the list of connected players, ensuring thread safety using a mutex.   |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* username: The username of the player to remove.                                                  |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void remove_player(char* username)
{
	pthread_mutex_lock(&mutex);

	for (int i = 0; i < player_count; i++)
	{
		if (strcmp(connected_players[i], username) == 0)
        {
			for (int j = i; j < player_count - 1; j++)
            {
				strcpy(connected_players[j], connected_players[j + 1]);
			}

			player_count--;
			break;
		}
	}

	pthread_mutex_unlock(&mutex);
}

//FUNCTION TO GET THE LIST OF CONNECTED PLAYERS
// |-----------------------------------------------------------------------------------------------------------|
// | Function: get_connected_players                                                                           |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Retrieves the list of connected players and sends it to all clients. Ensures thread safety   |
// | using a mutex.                                                                                            |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* response: Buffer to store the response string.                                                   |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void get_connected_players(char* response)
{
	pthread_mutex_lock(&mutex);

	if (player_count == 0)
    {
		strcpy(response, "7/0\n");
	}
	else
    {
		strcpy(response, "7/");
		
		for (int i = 0; i < player_count; i++) //para cada posición
		{
            int current = 0;
            int encontrado = 0;
            while (current < i)
            {
                if (strcmp(connected_players[current], connected_players[i]) == 0)
                {
                    encontrado = 1;
                }
                current = current + 1;
            }
            if (encontrado == 0) //si no se ha encontrado
			{
                strcat(response, connected_players[i]);
			    strcat(response, "-");
			    printf("Respuesta: ");
			    printf(response);
			    printf("\n");
            }
		}
		
		for (int j=0; j<client_count; j++) {
			
			if (sockets[j] != NULL){
				write(sockets[j]->sock_conn, response, strlen(response)); // Enviar el mensaje a cada cliente
			}
		}
	}

	pthread_mutex_unlock(&mutex);
}

// CLIENT'S REQUEST
// |-----------------------------------------------------------------------------------------------------------|
// | Function: process_request                                                                                 |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Processes client requests by parsing the input and invoking appropriate functions based on   |
// | the request code.                                                                                         |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* request: The incoming request string.                                                            |
// |  - char* response: Buffer to store the response string.                                                   |
// |  - MYSQL* conn: Pointer to the MySQL connection.                                                          |
// |  - char* username: The username of the client sending the request.                                        |
// |  - int sender_sock: The socket descriptor of the client sending the request.                              |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void process_request(char* request, char* response, MYSQL* conn, char* username, int sender_sock)
{
    char* p = strtok(request, "/");
    int codigo = atoi(p);
	char password[60];
	char chat_message[200];
    int numero;
	
    switch (codigo)
    {
        case 1: //REGISTER
			p = strtok(NULL, "/");
			if (p == NULL)
            {
				strcpy(response, "Invalid request format for registration\n");
				return;
			}
			strcpy(username, p);
			p = strtok(NULL, "/");
			if (p == NULL)
            {
				strcpy(response, "Invalid request format for registration\n");
				return;
			}
			strcpy(password, p);
			register_user(username, password, response, conn);
			break;

        case 2: //LOGIN
			p = strtok(NULL, "/");
			if (p == NULL)
            {
				strcpy(response, "Invalid request format for login\n");
				return;
			}
			strcpy(username, p);
			p = strtok(NULL, "/");
			if (p == NULL)
            {
				strcpy(response, "Invalid request format for login\n");
				return;
			}
			strcpy(password, p);
			login_user(username, password, response, conn);
			break;
            
       case 3: // Delete user
			if (username[0] == '\0') {
				strcpy(response, "3/0");
				return;
			}
			remove_player(username);
			delete_user(username, response, conn);
			get_connected_players(response);
			break;
		
		case 4: // Players with whom I have played
			if (username[0] == '\0') {
				strcpy(response, "Player not specified\n");
				return;
			}
			get_players_played_with(username, response, conn);
			break;

		case 7: //CONNECTED PLAYERS LIST
			printf("Request received for listing connected players\n");
			get_connected_players(response);
			break;
			
		case 100: //CHAT
			p = strtok(NULL, "/");
			if (p == NULL) {
				strcpy(response, "Invalid request format for chat\n");
				return;
			}
			strcpy(username, p);
			p = strtok(NULL, "/");
			if (p == NULL) {
				strcpy(response, "Invalid request format for chat\n");
				return;
			}
			strcpy(chat_message, p);
			broadcast_message_chat(chat_message,username,response, sender_sock);
			break;

		case 200: //Create a new game
			p = strtok(NULL, "/");
			if (p == NULL) {
				strcpy(response, "Invalid request format for game creation\n");
				return;
			}
			strcpy(username, p);
			//printf("CASO 200\n");
			create_new_game(username, response, conn, sender_sock);
			break;

		case 210: //Process Invite
			p = strtok(NULL, "/");
			printf(p);
			printf("\n");
			invite_player(p);

			break;

		case 220: //Player is joining
			p = strtok(NULL, "/");
			printf(p);
			printf("\n");
			joinGame(p, response, conn, sender_sock);

			break;

		case 230: //Ready to start the game
			p = strtok(NULL, "/");
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				readyToStartGame(p, response, conn, sender_sock);
			}
			printf("FINAL 230\n");

			break;

		case 300: //Asked for current PlayerTurn
			p = strtok(NULL, "/"); //p = GameID from wich the current turn is beeingRequested
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				requestPlayerTurn(p, response, conn);
			}
			printf("FINAL 300\n");

			break;

        case 400: //Asked for current TokenVector
			p = strtok(NULL, "/"); //p = GameID from wich the tokenVector is beeingRequested
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				sendCurrentTokenVector(p, response, conn, sender_sock);
			}
			printf("FINAL 400\n");

			break;

        case 500: //UPDATE Positions
            p = strtok(NULL, "/"); //p = GameID|PosicionFichas
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				updateTokenVector(p, response, conn, sender_sock);
			}
			printf("FINAL 500\n");

            break;

        case 900: //GameEnd
            p = strtok(NULL, "/"); //p = GameID|NombreGanador|Puntos
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				endGame(p, response, conn, sender_sock);
			}
			printf("FINAL 900\n");

            break;

        case 1000: //list games in a time period
            p = strtok(NULL, "/"); //p = fechaInicio|fechaFinal

            if (p != NULL)
			{
				listGames(p, response, conn, sender_sock);
			}
			printf("FINAL 900\n");

            break;
		
		case 0:
			remove_player(username);
            get_connected_players(response);
			strcpy(response, "End of connection\n");
            
            break;

		default:
            strcpy(response, "Invalid code\n");
			break;
    }
}

// CLIENT
// |-----------------------------------------------------------------------------------------------------------|
// | Function: handle_client                                                                                   |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Handles communication with the client, processing requests and sending responses.            |
// | Manages the client session in a separate thread until the client disconnects.                             |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - void* data: A pointer to a `thread_data` structure containing the client's socket, username, and MySQL |
// |    connection.                                                                                            |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void* handle_client(void* data) {
	struct thread_data* t_data = (struct thread_data*)data;
	int sock_conn = t_data->sock_conn;
    t_data->enPartida = 0;
	MYSQL* conn = t_data->conn;
	
	char request[512];
	char response[512];
	int ret;
	int terminar = 0;
	
	while (!terminar) {
		ret = read(sock_conn, request, sizeof(request) -1);
		request[ret] = '\0';

		printf("Received request: %s\n", request);
        printf(request);
        printf("\n");
		
		process_request(request, response, conn, t_data->username,sock_conn);
        write(sock_conn, response, strlen(response));
		
		if (strcmp(request, "0") == 0) {
			terminar = 1;
		}
	}
	close(sock_conn);
	free(t_data); //freeinf memory of the thread data 
	pthread_exit(NULL); //end the thread
}

// GET PLAYERS PLAYED WITH
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: get_players_played_with                                                                         | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Retrieves a list of players with whom the specified user has played. It queries the database | 
// | to find players that shared the same game sessions but were not the current player. The list of player is | 
// | returned in the response buffer.                                                                          | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input: | // | - char* username: The username of the current player.                                       | 
// | - char* response: Buffer to store the response string with the list of players.                           | 
// | - MYSQL* conn: Pointer to the MySQL connection.                                                           | 
// | Output: None.                                                                                             | 
// |-----------------------------------------------------------------------------------------------------------|
void get_players_played_with(char* username, char* response, MYSQL* conn) {
	char query[400];
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	
	sprintf(query, "SELECT DISTINCT Users.Username FROM Game_Members AS CurrentPlayerGame JOIN Game_Members AS OtherPlayerGame ON CurrentPlayerGame.Game_ID = OtherPlayerGame.Game_ID JOIN Users ON OtherPlayerGame.Player_ID = Users.Player_ID WHERE CurrentPlayerGame.Player_ID = (SELECT Player_ID FROM Users WHERE Username = '%s') AND CurrentPlayerGame.Player_ID != OtherPlayerGame.Player_ID;", username);
	if (mysql_query(conn, query)) {
		strcpy(response, "Error en la consulta\n");
		return;
	}
	
	resultado = mysql_store_result(conn);
	
	if (mysql_num_rows(resultado) > 0) {
		strcpy(response, "4/1/");
		
		while ((row = mysql_fetch_row(resultado))) {
			strcat(response, row[0]);
			strcat(response, "/");
		}
	}
	else 
	{
		strcpy(response, "4/0\n");
	}
	
	write(conn, response, strlen(response));
	
	mysql_free_result(resultado);
	
	printf("Respuesta: %s\n", response);
}

// DELETE USER
// |-----------------------------------------------------------------------------------------------------------|
// | Function: delete_user                                                                                     |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Deletes a user by marking their username and password as deleted in the database. Removes    |
// | the user from the connected players list.                                                                 |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - const char* username: The username of the user to delete.                                              |
// |  - char* response: Buffer to store the response string.                                                   |
// |  - MYSQL* conn: Pointer to the MySQL connection.                                                          |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void delete_user(const char* username, char* response, MYSQL* conn) {
	char query[512];
	
	snprintf(query, sizeof(query),
			 "UPDATE Users SET Username = 'UserDeleted', Password = 'PasswordDeleted' WHERE Username = '%s';", username);

	if (mysql_query(conn, query) == 0) 
	{
		remove_player(username);
		snprintf(response, 512, "3/1/User '%s' deleted successfully\n", username);
		printf("User '%s' deleted from database and removed from the connected list.\n", username);
	}
	else
	{
		snprintf(response, 512, "3/0/Error deleting user: %s\n", mysql_error(conn));
		printf("Error deleting user '%s': %s\n", username, mysql_error(conn));
	}
}

// REGISTER A NEW USER
// |-----------------------------------------------------------------------------------------------------------|
// | Function: register_user                                                                                   |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Registers a new user by checking if the username exists and inserting the user into the      |
// | database if it does not.                                                                                  |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* username: The username to register.                                                              |
// |  - char* password: The password for the new user.                                                         |
// |  - char* response: Buffer to store the response string.                                                   |
// |  - MYSQL* conn: Pointer to the MySQL connection.                                                          |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void register_user(char* username, char* password, char* response, MYSQL* conn) {
	char query[512];
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	
	// check if the username already exists
	snprintf(query, sizeof(query),
			 "SELECT Username FROM Users WHERE Username='%s'", username);
	
	if (mysql_query(conn, query) != 0) {
		snprintf(response, 512, "1/3",
				 mysql_errno(conn), mysql_error(conn));
		return;
	}
	resultado = mysql_store_result(conn);
	if (resultado == NULL) {
		snprintf(response, 512, "1/3",
				 mysql_errno(conn), mysql_error(conn));
		return;
	}
	
	if (mysql_num_rows(resultado) > 0) {
		strcpy(response, "1/0");
		mysql_free_result(resultado);
		return;
	}
	mysql_free_result(resultado);
	
	// Insert the new user
	snprintf(query, sizeof(query),
			 "INSERT INTO Users (Username, Password) VALUES ('%s', '%s');", username, password);
	if (mysql_query(conn, query) == 0) {
		// Obtain the ID
		my_ulonglong inserted_id = mysql_insert_id(conn);
		snprintf(response, 512, "1/1");
		add_player(username);
	} else {
		snprintf(response, 512, "1/3",
				 mysql_errno(conn), mysql_error(conn));
	}
}
	
// LOGIN AN EXISTING USER
// |-----------------------------------------------------------------------------------------------------------|
// | Function: login_user                                                                                      |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Authenticates a user by checking the provided credentials against the database.              |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - char* username: The username to authenticate.                                                          |
// |  - char* password: The password for the user.                                                             |
// |  - char* response: Buffer to store the response string.                                                   |
// |  - MYSQL* conn: Pointer to the MySQL connection.                                                          |
// | Output: None.                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
void login_user(char* username, char* password, char* response, MYSQL* conn) {
    char query[400];
    MYSQL_RES* resultado;

    sprintf(query, "SELECT Username, Password FROM Users WHERE Username='%s' AND Password='%s'", username, password);
    mysql_query(conn, query);
    resultado = mysql_store_result(conn);
	

    if (mysql_num_rows(resultado) == 1) //Si coinciden con BBDD
    {
        strcpy(response, "2/1\n");
		add_player(username);
		printf("%s logged in and added to the list\n", username);
	}
    else //No coincide con BBDD
    {
        strcpy(response, "2/0\n");
    }
    mysql_free_result(resultado);
}

//CREATE A NEW GAME REGISTER
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: create_new_game                                                                                 | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Creates a new game record in the database and registers the current player as the game admin.| 
// | The function inserts a new game into the Game table, retrieves the game ID, and inserts the               | 
// | current player into the Game_Members table. The game ID is then sent back to the client.                  | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input:                                                                                                    | 
// | - char* username: The username of the player creating the game.                                           | 
// | - char* response: Buffer to store the response string containing the game ID.                             | 
// | - MYSQL* conn: Pointer to the MySQL connection used to interact with the database.                        | 
// | - int SenderSockID: The socket ID of the sender for identifying the player to update their game info.     | 
// | Output: None.                                                                                             | 
// |-----------------------------------------------------------------------------------------------------------|
void create_new_game(char* username, char* response, MYSQL* conn, int SenderSockID)
{
    printf("ENTRADO EN CREACION JUEGO\n");

    char query0[400];
    MYSQL_RES* resultado;
    MYSQL_ROW row;

    // Construcción de la consulta SELECT para obtener Player_ID
    snprintf(query0, sizeof(query0), "SELECT Users.Player_ID FROM Users WHERE Users.Username = '%s';", username);

    // Ejecución de la consulta SELECT
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error en la consulta SELECT: %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados de la consulta SELECT
    resultado = mysql_store_result(conn);
    if (resultado == NULL) {
        fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
        return;
    }

    // Obtener la primera fila de los resultados
    row = mysql_fetch_row(resultado);
    if (row == NULL) {
        fprintf(stderr, "No se encontró el usuario con el nombre proporcionado.\n");
        mysql_free_result(resultado);
        return;
    }

    // Obtener el Player_ID (suponiendo que es un valor numérico)
    int player_id = atoi(row[0]);  // Convertimos el Player_ID de string a int
    printf("Player_ID: %d\n", player_id);

    // Limpiar resultados de la consulta SELECT de Player_ID
    mysql_free_result(resultado);

    // Ahora insertamos el registro en la tabla Game
    char query1[400];
    snprintf(query1, sizeof(query1), "INSERT INTO Game (GameStatus, PosicionFichas, GameAdmin, GameTime) VALUES (2, '0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0', %d, NOW());", player_id);

    // Ejecutar la consulta INSERT
    if (mysql_query(conn, query1)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }
    printf("Juego creado con éxito. ID de juego insertado.\n");

    // Conseguimos el identificador del juego insertado
    char query2[400];
    MYSQL_RES* resultado2;
    MYSQL_ROW row2;

    // Construcción de la consulta SELECT para obtener el MAX(Game_ID)
    snprintf(query2, sizeof(query2), "SELECT MAX(Game_ID) FROM Game;");

    // Ejecutar la consulta SELECT para obtener el MAX(Game_ID)
    if (mysql_query(conn, query2)) {
        fprintf(stderr, "Error en la consulta SELECT MAX(Game_ID): %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados de la consulta SELECT
    resultado2 = mysql_store_result(conn);
    if (resultado2 == NULL) {
        fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
        return;
    }

    // Obtener la primera fila de los resultados
    row2 = mysql_fetch_row(resultado2);
    if (row2 == NULL || row2[0] == NULL) {
        fprintf(stderr, "No se encontraron registros en la tabla Game.\n");
        mysql_free_result(resultado2);
        return;
    }

    // Obtener el valor de MAX(Game_ID) y convertirlo a entero
    int max_game_id = atoi(row2[0]);  // Convertir el valor de MAX(Game_ID) a int
    printf("El valor máximo de Game_ID es: %d\n", max_game_id);

    // Limpiar los resultados de la consulta SELECT para MAX(Game_ID)
    mysql_free_result(resultado2);

    // Insertar el registro en la tabla Game_Members
    char query3[400];
    snprintf(query3, sizeof(query3), "INSERT INTO Game_Members (Game_ID, Player_ID, PlayerNumber) VALUES (%d, %d, 1);", max_game_id, player_id);

    // Ejecutar la consulta INSERT en Game_Members
    if (mysql_query(conn, query3)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    printf("Registro insertado correctamente en Game_Members.\n");
	memset(response, 0, sizeof(response));  // Vacía el contenido de response
	snprintf(response, sizeof(response), "%s", "201/");

	// Luego, concatenamos el valor de max_game_id a la respuesta
	snprintf(response + strlen(response), sizeof(response) - strlen(response), "%d", max_game_id);

	int currentSearchingSocket = 0;
	bool encontrado = false;

	while ((sockets[currentSearchingSocket] != NULL)&&(!encontrado))
	{
		if (sockets[currentSearchingSocket]->sock_conn == SenderSockID)
		{
			sockets[currentSearchingSocket]->IdentificadorJugadorPartida = 1;
			//printf("PONGO IDJP\n");
			sockets[currentSearchingSocket]->IdentificadorPartida = max_game_id;
            sockets[currentSearchingSocket]->enPartida = 1;
			encontrado = true;
			//printf("PONGO IP\n");
		}
		currentSearchingSocket++;
	}

	printf("\n");
	
	printf("ENVIADO 201\n");
	printf(response);
	printf("\n");
}

// INVITE PLAYER
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: invite_player                                                                                   | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Handles sending an invitation to a specified player. It extracts the invited player's        | 
// | username from the message, finds the corresponding socket, and sends the invitation message to that       | 
// | player's socket if it is valid. Logs the process for debugging purposes.                                  | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input:                                                                                                    | 
// | - char* mensajer: The message containing the invitation details, including the invited player's username. | 
// | Output: Sends the invitation message to the appropriate player's socket and logs success or failure.      | 
// |-----------------------------------------------------------------------------------------------------------|
void invite_player(char* mensajer)
{
    printf("ENTRADO INVITACION\n");
    printf("%s\n", mensajer);

    // Definir variables necesarias
    char* jugadorinvitado = NULL;
    bool socketEncontrado = false;
    int currentSearchingSocket = 0;

    // Definir una variable intermedia para almacenar el mensaje que se va a reenviar
    char mensaje_intermedio[1024];
    strncpy(mensaje_intermedio, mensajer, sizeof(mensaje_intermedio) - 1);  // Copiar mensaje a la variable intermedia
    mensaje_intermedio[sizeof(mensaje_intermedio) - 1] = '\0';  // Asegurarse de que esté correctamente terminado en null

    // Saltar el primer token (id respuesta) con strtok
    jugadorinvitado = strtok(mensajer, "-");  // Obtener primer token, se descarta

    // Obtener el tercer token (jugadorinvitado)
    jugadorinvitado = strtok(NULL, "-");  // Obtener el tercer token

    // Verificar si se obtuvo un jugador invitado
    if (jugadorinvitado == NULL) {
        printf("Error: No se encontró el jugador invitado en el mensaje.\n");
        return;
    }

    // Bucle para buscar el socket correspondiente
    while (currentSearchingSocket < player_count && !socketEncontrado)
    {
        if (strcmp(sockets[currentSearchingSocket]->username, jugadorinvitado) == 0)
        {
            if (sockets[currentSearchingSocket]->enPartida == 0) //si no esta en partida se lo mando a esa sesión
            {
                socketEncontrado = true;
                // Asegurarse de que el socket sea válido antes de escribir
                if (sockets[currentSearchingSocket]->sock_conn > 0) {
                    // Formatear el mensaje con snprintf
                    char result_invite[1024];
                    snprintf(result_invite, sizeof(result_invite), "211/%s", mensaje_intermedio);

                    // Enviar el mensaje al socket
                    write(sockets[currentSearchingSocket]->sock_conn, result_invite, strlen(result_invite));
                    printf("Invitación enviada a %s.\n", jugadorinvitado);
                    printf("El jugador que ha sido invitado recibirá: %s\n", result_invite);  // Mostrar el mensaje que se enviará
                }
                else
                {
                    printf("Error: Socket inválido para el jugador %s.\n", jugadorinvitado);
                }
            }
        }
        currentSearchingSocket++;
    }

    if (!socketEncontrado) {
        printf("Jugador %s no encontrado.\n", jugadorinvitado);
    }
}

// JOIN GAME
// |-----------------------------------------------------------------------------------------------------------|
// | Function: joinGame                                                                                        |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Allows a user to join a game by validating the input, updating the database, and notifying   |
// |              all players in the game of the updated player list.                                          |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// | - const char* input: The input string containing the username and game ID, separated by '-'.              |
// | - char* response: A buffer used for formatting responses and notifications.                               |
// | - MYSQL* conn: A pointer to the MySQL connection object used for executing SQL queries.                   |
// | - int SenderSockID: The socket descriptor of the sender, used to associate the player with the socket.    |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// | - Adds the player to the game in the database.                                                            |
// | - Updates the player's socket information to reflect their game and player ID.                            |
// | - Notifies all players in the game with the updated list of usernames.                                    |
// |-----------------------------------------------------------------------------------------------------------|
void joinGame(const char* input, char* response, MYSQL* conn, int SenderSockID)
{
    printf("ENTRANDO EN UNIRSE A PARTIDA\n");

    // Validación de parámetros
    if (input == NULL || conn == NULL) {
        fprintf(stderr, "Parámetros inválidos.\n");
        return;
    }

    // Copiar el input porque strtok modifica la cadena
    char input_copy[200];
    strncpy(input_copy, input, sizeof(input_copy) - 1);
    input_copy[sizeof(input_copy) - 1] = '\0';

    // Separar `username` y `game_id`
    char* usernamedado = strtok(input_copy, "-");
    char* game_id_str = strtok(NULL, "-");

    if (usernamedado == NULL || game_id_str == NULL) {
        fprintf(stderr, "Formato inválido de entrada: %s. Se esperaba 'username-idpartida'.\n", input);
        return;
    }

    printf("GAME ID: %s\n", game_id_str);
    printf("User to invite: %s\n", usernamedado);

    int game_id = atoi(game_id_str); // Convertir el ID de partida a entero


    // Variables para las consultas
    char query[400];
    MYSQL_RES* resultado;
    MYSQL_ROW row;

    // Construir la consulta
    snprintf(query, sizeof(query), "SELECT Users.Player_ID FROM Users WHERE Users.Username = '%s';", usernamedado);

    //printf("Consulta: %s\n", query);

    // Ejecución de la consulta SELECT
    if (mysql_query(conn, query)) {
        fprintf(stderr, "Error en la consulta SELECT: %s\n", mysql_error(conn));
        return;
    }
    // Obtener los resultados de la consulta SELECT
    resultado = mysql_store_result(conn);
    if (resultado == NULL) {
        fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
        return;
    }

    // Obtener la primera fila de los resultados
    row = mysql_fetch_row(resultado);
    if (row == NULL) {
        fprintf(stderr, "No se encontró el usuario con el nombre proporcionado.\n");
        mysql_free_result(resultado);
        return;
    }
	// Obtener el Player_ID (suponiendo que es un valor numérico)
    int player_id = atoi(row[0]);  // Convertimos el Player_ID de string a int
    printf("Player_ID: %d\n", player_id);

    // Obtener el PlayerNumber máximo de la partida
    snprintf(query, sizeof(query), 
             "SELECT MAX(PlayerNumber) FROM Game_Members WHERE Game_ID = %d;", game_id);

    if (mysql_query(conn, query)) {
        fprintf(stderr, "Error en la consulta SELECT MAX(PlayerNumber): %s\n", mysql_error(conn));
        return;
    }

    resultado = mysql_store_result(conn);
    if (resultado == NULL) {
        fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
        return;
    }

    row = mysql_fetch_row(resultado);
    int player_number = (row && row[0]) ? atoi(row[0]) + 1 : 1;
    mysql_free_result(resultado); // Limpiar resultados de la segunda consulta

    // Verificar si hay espacio en la partida
    if (player_number > 4) {
        printf("JUEGO LLENO\n");
        return;
    }
    // Insertar el registro en Game_Members
    snprintf(query, sizeof(query), 
             "INSERT INTO Game_Members (Game_ID, Player_ID, PlayerNumber) "
             "VALUES (%d, %d, %d);", game_id, player_id, player_number);

    if (mysql_query(conn, query)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    printf("Registro insertado correctamente en Game_Members.\n");

    // Consulta para obtener los usernames de los jugadores en el juego dado
    snprintf(query, sizeof(query), 
             "SELECT Users.Username FROM Game_Members "
             "JOIN Users ON Game_Members.Player_ID = Users.Player_ID "
             "WHERE Game_Members.Game_ID = %d;", game_id);

    printf("Consulta: %s\n", query);

    // Ejecutar la consulta
    if (mysql_query(conn, query)) {
        fprintf(stderr, "Error en la consulta SELECT Game_Members: %s\n", mysql_error(conn));
        return;
    }

    // Almacenar el resultado de la consulta
    resultado = mysql_store_result(conn);
    if (resultado == NULL) {
        fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
        return;
    }

    // Crear una cadena para almacenar todos los usernames concatenados
    char usernames[1000] = "";  // Asegúrate de que el tamaño sea suficiente para almacenar los usernames

    // Recorrer las filas de resultados
    while ((row = mysql_fetch_row(resultado))) {
        // Concatenar el username a la cadena, separándolos por "-"
        if (strlen(usernames) > 0) {
            strcat(usernames, "-");  // Agregar el separador "-"
        }
        strcat(usernames, row[0]);  // Concatenar el username
    }

    // Mostrar el resultado
    printf("Usernames del juego (Game_ID = %d): %s\n", game_id, usernames);

    // Liberar recursos
    mysql_free_result(resultado);

	//MODIFICAR INFORMACION ADICIONAL LISTA SOCKETS
	int currentSearchingSocket = 0;
	bool encontrado = false;

	while ((sockets[currentSearchingSocket] != NULL)&&(!encontrado))
	{
		if (sockets[currentSearchingSocket]->sock_conn == SenderSockID)
		{
			sockets[currentSearchingSocket]->IdentificadorJugadorPartida = player_number;
			//printf("PONGO IDJP\n");
			sockets[currentSearchingSocket]->IdentificadorPartida = game_id;
            sockets[currentSearchingSocket]->enPartida = 1;
			encontrado = true;
			//printf("PONGO IP\n");
		}
		currentSearchingSocket++;
	}

	//MANDAR A SOCKETS DE PARTIDA
	currentSearchingSocket = 0;
	while (currentSearchingSocket < player_count)
	{
        if (sockets[currentSearchingSocket]->IdentificadorPartida == game_id)
		{
            
            char result_invite[1024];
            snprintf(result_invite, sizeof(result_invite), "221/%s", usernames);

            // Enviar el mensaje al socket
            write(sockets[currentSearchingSocket]->sock_conn, result_invite, strlen(result_invite));
			//printf("ENVIADO A PLAYER");
        }
        currentSearchingSocket++;
    }

}


// START GAME
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: readyToStartGame                                                                                | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Marks a game as ready to start by updating the game status in the database. Receives a game  | 
// | ID as input and updates the corresponding entry in the `Game` table. Logs the process for debugging.      | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input: | // | - const char* input: The game ID as a string to identify the game to be updated.            | 
// | - char* response: A buffer to store potential error messages or results.                                  | 
// | - MYSQL* conn: The MySQL connection object for executing the database update.                             | 
// | - int SenderSockID: The socket ID of the sender requesting the update.                                    | 
// | Output: Updates the `Game` table's status for the specified game ID. Logs success or failure messages.    | 
// |-----------------------------------------------------------------------------------------------------------|
void readyToStartGame(const char* input, char* response, MYSQL* conn, int SenderSockID)
{
	printf("GAME ");
	printf(input);
	printf(" REQUESTED TO START\n");

	// Insertar el registro en la tabla Game_Members
    char query0[400];
    snprintf(query0, sizeof(query0), 
             "UPDATE Game SET GameStatus = %d WHERE Game_ID = %d;", 
             101, atoi(input));

    // Ejecutar la consulta INSERT en Game_Members
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    printf("Estado de partida actualizado a 101\n");

	//MANDAR A SOCKETS DE PARTIDA
	int currentSearchingSocket = 0;
	while (currentSearchingSocket < player_count)
	{
        if (sockets[currentSearchingSocket]->IdentificadorPartida == atoi(input))
		{
            
            char result_startGame[1024];
            snprintf(result_startGame, sizeof(result_startGame), "231/");

            // Enviar el mensaje al socket
            write(sockets[currentSearchingSocket]->sock_conn, result_startGame, strlen(result_startGame));
        }
        currentSearchingSocket++;
    }

	sendPositionsToGame(atoi(input), response, conn);

}

// SEND POSITIONS
// |-----------------------------------------------------------------------------------------------------------|
// | Function: sendPositionsToGame                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Sends the game piece positions (`PosicionFichas`) of a specific game to all connected        |
// | clients who are part of that game. Queries the database for the positions and sends them to the sockets   |
// | associated with the specified game.                                                                       |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - int gameIDToSend: The ID of the game whose positions need to be sent.                                  |
// |  - char* response: A buffer for potential error messages or additional information.                       |
// |  - MYSQL* conn: The MySQL connection object for executing database queries.                               |
// | Output: Sends the formatted positions to all connected clients in the specified game and logs the         |
// | process.                                                                                                  |
// |-----------------------------------------------------------------------------------------------------------|
void sendPositionsToGame(int gameIDToSend, char* response, MYSQL* conn)
{
	printf("ENTERED TO SEND POSITIONS TO GAME");
	char query[256];
    MYSQL_RES *result;
    MYSQL_ROW row;

    // Construir la consulta SQL
    snprintf(query, sizeof(query), "SELECT Game.PosicionFichas FROM Game WHERE Game.Game_ID = %d;", gameIDToSend);

    // Ejecutar la consulta
    if (mysql_query(conn, query)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados
    result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
        return;
    }

    // Procesar los resultados
    if ((row = mysql_fetch_row(result)) != NULL) {
        printf("PosicionFichas para Game_ID %d: %s\n", gameIDToSend, row[0]);
    } else {
        printf("No se encontró ningún resultado para Game_ID %d\n", gameIDToSend);
    }

	//MANDAR A SOCKETS DE PARTIDA
	int currentSearchingSocket = 0;
	while (currentSearchingSocket < player_count)
	{
        if (sockets[currentSearchingSocket]->IdentificadorPartida == gameIDToSend)
		{
            
            char result_sendPosition[1024];
            snprintf(result_sendPosition, sizeof(result_sendPosition), "241/%s-", row[0]);


            // Enviar el mensaje al socket
            write(sockets[currentSearchingSocket]->sock_conn, result_sendPosition, strlen(result_sendPosition));
        }
        currentSearchingSocket++;
    }

	// Liberar los resultados
    mysql_free_result(result);

}

// PLAYER TURN
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: requestPlayerTurn                                                                               | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Queries the current status of a game from the database based on the provided game ID and     | 
// | prepares the response with the game status to be sent to the requesting client.                           | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input: | // | - const char* currentGivenGameID: The ID of the game whose status is being requested.       | 
// | - char* response: A buffer to store the formatted game status to send back.                               | 
// | - MYSQL* conn: The MySQL connection object for executing database queries.                                | 
// | Output: Updates the response buffer with the formatted game status and logs the process.                  | 
// |-----------------------------------------------------------------------------------------------------------|
void requestPlayerTurn(const char* currentGivenGameID, char* response, MYSQL* conn)
{
    // Imprimir los resultados
    printf("currentGameID: %s\n", currentGivenGameID);


	char query0[256];
    MYSQL_RES *result;
    MYSQL_ROW row;

    // Construir la consulta SQL
    snprintf(query0, sizeof(query0), "SELECT Game.GameStatus FROM Game WHERE Game.Game_ID = %d;", atoi(currentGivenGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados
    result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
        return;
    }

    // Procesar los resultados
    if ((row = mysql_fetch_row(result)) != NULL) {
        printf("Status para Game_ID %d: %s\n", atoi(currentGivenGameID), row[0]);
    } else {
        printf("No se encontró ningún resultado para Game_ID %d\n", atoi(currentGivenGameID));
    }

    char result_GameStatus[255];
    snprintf(result_GameStatus, sizeof(result_GameStatus), "301/%s", row[0]);

    printf("A ENVIAR:\n");
    printf(result_GameStatus);
    printf("\n");

    strcpy(response, result_GameStatus);

    printf("ENVIADO 300\n");

}

// CURRENT TOKEN VECTOR
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: sendCurrentTokenVector                                                                          | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Queries the database for the current token positions (`PosicionFichas`) of a specific game   | 
// | and sends the formatted result back to the requesting client. Logs the query and response process.        | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input: | // | - char* currentGivenGameID: The ID of the game whose token positions are requested.         | 
// | - char* response: A buffer to store the formatted token positions to send back.                           | 
// | - MYSQL* conn: The MySQL connection object for executing database queries.                                | 
// | - int sender_sock: The socket descriptor of the client requesting the token vector.                       | 
// | Output: Updates the response buffer with the formatted token positions and logs the process.              | 
// |-----------------------------------------------------------------------------------------------------------|
void sendCurrentTokenVector(char* currentGivenGameID, char* response, MYSQL* conn, int sender_sock) //400
{
    // Imprimir los resultados
    printf("currentGameID: %s\n", currentGivenGameID);


	char query0[256];
    MYSQL_RES *result;
    MYSQL_ROW row;

    // Construir la consulta SQL
    snprintf(query0, sizeof(query0), "SELECT Game.PosicionFichas FROM Game WHERE Game.Game_ID = %d;", atoi(currentGivenGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados
    result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
        return;
    }

    // Procesar los resultados
    if ((row = mysql_fetch_row(result)) != NULL) {
        printf("PosicionFichas para Game_ID %d: %s\n", atoi(currentGivenGameID), row[0]);
    } else {
        printf("No se encontró ningún resultado para Game_ID %d\n", atoi(currentGivenGameID));
    }

    char result_QueryTokenVector[255];
    snprintf(result_QueryTokenVector, sizeof(result_QueryTokenVector), "401/%s", row[0]);

    printf("A ENVIAR:\n");
    printf(result_QueryTokenVector);
    printf("\n");

    strcpy(response, result_QueryTokenVector);

    printf("ENVIADO 400\n");
}

// UPDATE TOKEN VECTOR
// |-----------------------------------------------------------------------------------------------------------|
// | Function: updateTokenVector                                                                               |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Updates the token positions of a game in the database, manages game status transitions,      |
// |              and notifies players of the next turn.                                                       |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// | - char* input: The input string containing the current game ID and updated token positions,               |
// |                separated by '|'.                                                                          |
// | - char* response: A buffer used for formatting responses and notifications.                               |
// | - MYSQL* conn: A pointer to the MySQL connection object used for executing SQL queries.                   |
// | - int sender_sock: The socket descriptor of the sender (not explicitly used in this function).            |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// | - Updates the database with the new token positions and game status.                                      |
// | - Notifies players in the game of the updated token positions and whose turn is next.                     |
// |-----------------------------------------------------------------------------------------------------------|
void updateTokenVector(char* input, char* response, MYSQL* conn, int sender_sock) //500
{
    char currentGameID[100];
    char givenVectorFichas[255];

    // Usamos strtok para separar la cadena
    char *token = strtok(input, "|");
    strcpy(currentGameID, token);  // El primer token es el currentGameID

    // El segundo token es el vector de posicion de fichas actualizado
    token = strtok(NULL, "|");
    strcpy(givenVectorFichas, token);  // Guardamos el segundo token


    // Prepara la consulta SQL con el nuevo valor de PosicionFichas y el Game_ID
    char query1[512];
    snprintf(query1, sizeof(query1), 
        "UPDATE Game SET PosicionFichas = '%s' WHERE Game_ID = %d;", 
        givenVectorFichas, atoi(currentGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query1)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }


    // Mostrar la consulta generada (esto es solo para ver cómo quedó la consulta)
    printf("Consulta SQL: %s\n", query1);
	sendPositionsToGame(atoi(currentGameID), response, conn);

    char query0[256];
    MYSQL_RES *result;
    MYSQL_ROW row;

    // Construir la consulta SQL
    snprintf(query0, sizeof(query0), "SELECT Game.GameStatus FROM Game WHERE Game.Game_ID = %d;", atoi(currentGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Obtener los resultados
    result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
        return;
    }

    // Procesar los resultados
    if ((row = mysql_fetch_row(result)) != NULL) {
        printf("Status para Game_ID %d: %s\n", atoi(currentGameID), row[0]);
    } else {
        printf("No se encontró ningún resultado para Game_ID %d\n", atoi(currentGameID));
    }

    int currentTurn = atoi(row[0]);
    int nextTurn = 101;
    if (currentTurn == 101)
    {
        nextTurn = 102;
    }
    else if (currentTurn == 102)
    {
        nextTurn = 103;
    }
    else if (currentTurn == 103)
    {
        nextTurn = 104;
    }
    else
    {
        nextTurn = 101;
    }
    // Insertar el registro en la tabla Game_Members
    char query2[400];
    snprintf(query2, sizeof(query2), 
             "UPDATE Game SET GameStatus = %d WHERE Game_ID = %d;", 
             nextTurn, atoi(currentGameID));

    // Ejecutar la consulta INSERT en Game_Members
    if (mysql_query(conn, query2)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    char nextTurnNotification[512];
    snprintf(nextTurnNotification, sizeof(nextTurnNotification), "351/%d",nextTurn);

    int currentSearchingSocket = 0;
	while (currentSearchingSocket < player_count)
	{
        if (sockets[currentSearchingSocket]->IdentificadorPartida == atoi(currentGameID))
		{
            printf(nextTurnNotification);
            printf("\n");
            // Enviar el mensaje al socket
            write(sockets[currentSearchingSocket]->sock_conn, nextTurnNotification, strlen(nextTurnNotification));
        }
        currentSearchingSocket++;
    }



}

// END GAME
// |-----------------------------------------------------------------------------------------------------------|
// | Function: endGame                                                                                         |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Finalizes a game by updating its status and result in the database, and notifies players.     |
// |              This function extracts game information from the input, updates the database, and broadcasts |
// |              the winner's name and end notification to all players in the game.                           |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// | - char* input: The input string containing the current game ID, winner's name, and score vector,          |
// |                separated by '|'.                                                                          |
// | - char* response: A buffer used for formatting responses and notifications.                               |
// | - MYSQL* conn: A pointer to the MySQL connection object used for executing SQL queries.                   |
// | - int sender_sock: The socket descriptor of the sender (not explicitly used in this function).            |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// | - Updates the database to set the game result and status to 'finished'.                                   |
// | - Sends the winner's name and end notification to all players in the game.                                |
// |-----------------------------------------------------------------------------------------------------------|
void endGame(char* input, char* response, MYSQL* conn, int sender_sock) //500
{
    char currentGameID[100];
    char WinnerName[100];
    char givenWinnerVector[255];

    // Usamos strtok para separar la cadena
    char *token = strtok(input, "|");
    strcpy(currentGameID, token);  // El primer token es el currentGameID

    // El segundo token es el nombre del jugador ganador
    token = strtok(NULL, "|");
    strcpy(WinnerName, token);  // Guardamos el segundo token

    // El tercer token es el vector de puntuaciones
    token = strtok(NULL, "|");
    strcpy(givenWinnerVector, token);  // Guardamos el tercer token


    // Prepara la consulta SQL con el nuevo valor de puntuaciones y el Game_ID
    char query1[512];
    snprintf(query1, sizeof(query1), 
        "UPDATE Game SET GameResult = '%s' WHERE Game_ID = %d;", 
        givenWinnerVector, atoi(currentGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query1)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Mostrar la consulta generada (esto es solo para ver cómo quedó la consulta)
    printf("Consulta SQL: %s\n", query1);
	sendPositionsToGame(atoi(currentGameID), response, conn);

    
    // Marcar estado de partida como terminado (900)
    char query2[400];
    snprintf(query2, sizeof(query2), 
             "UPDATE Game SET GameStatus = %d WHERE Game_ID = %d;", 
             900, atoi(currentGameID));

    // Ejecutar la consulta INSERT en Game_Members
    if (mysql_query(conn, query2)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    char endNotification[255];
    snprintf(endNotification, sizeof(endNotification), 
             "901/%s", WinnerName);

    int currentSearchingSocket = 0;
	while (currentSearchingSocket < player_count)
	{
        if (sockets[currentSearchingSocket]->IdentificadorPartida == atoi(currentGameID))
		{
            printf(endNotification);
            printf("\n");
            // Enviar el mensaje al socket
            write(sockets[currentSearchingSocket]->sock_conn, endNotification, strlen(endNotification));
            sockets[currentSearchingSocket]->enPartida = 0;
        }
        currentSearchingSocket++;
    }

}

// LIST GAMES
// |-----------------------------------------------------------------------------------------------------------|
// | Function: listGames                                                                                       |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: This function processes a request to list games within a specified date range. It executes a |
// | SQL query to retrieve game data from the database and formats the results into a response string to be    |
// | sent back to the client. It handles the extraction of date range information from the input and performs  |
// | multiple SQL queries to fetch game details and associated admin usernames.                                |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - input: A pointer to a character array containing the date range for the query in the format            |
// |           "yyyy-MM-dd HH:mm:ss|yyyy-MM-dd HH:mm:ss".                                                      |
// |  - response: A pointer to a character array where the result will be stored, formatted as a string.       |
// |  - conn: A pointer to the MYSQL connection object used to interact with the database.                     |
// |  - sender_sock: The socket of the sender (not used in this function but included in the parameters).      |
// | Output:                                                                                                   |
// |  - Sends back a formatted response string to the response pointer, containing the list of games in the    |
// |    specified date range, or a message indicating no results found.                                        |
// |-----------------------------------------------------------------------------------------------------------|
void listGames(char* input, char* response, MYSQL* conn, int sender_sock) //500
{
    char fechaInicio[100];
    char fechaFinal[100];

    // Usamos strtok para separar la cadena
    char *token = strtok(input, "|");
    strcpy(fechaInicio, token);  // El primer token es fechaInicio

    // El segundo token es fechaFinal
    token = strtok(NULL, "|");
    strcpy(fechaFinal, token);  // Guardamos el segundo token

    // Prepara la consulta SQL
    char query1[512];
    MYSQL_RES *result;
    MYSQL_ROW row;

    snprintf(query1, sizeof(query1), 
        "SELECT Game_ID, GameAdmin, GameTime FROM Game WHERE GameTime BETWEEN '%s' AND '%s'", fechaInicio, fechaFinal);

    // Ejecutar la consulta
    if (mysql_query(conn, query1)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }

    // Mostrar la consulta generada (esto es solo para ver cómo quedó la consulta)
    printf("Consulta SQL: %s\n", query1);

    // Obtener los resultados
    result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
        return;
    }

    // Comprobamos si hay resultados y los mostramos
    int num_rows = mysql_num_rows(result); // Número total de filas

    if (num_rows == 0)
    {
        printf("No se encontraron eventos en el rango de fechas especificado.\n");
        strcpy(response, "1001/EMPTY");
    }
    else
    {
        printf("Eventos entre %s y %s:\n", fechaInicio, fechaFinal);
        // Iterar sobre todos los resultados

        char textoResultado [2048]; // Inicializamos la cadena
        snprintf(textoResultado, sizeof(textoResultado), "1001/");

        int contadorConstruir = 0;
        while ((row = mysql_fetch_row(result)) != NULL)
        {
            if (contadorConstruir == 1)
            {
                snprintf(textoResultado + strlen(textoResultado), sizeof(textoResultado) - strlen(textoResultado), "|");
            }

            contadorConstruir = 1;

            char idPartidaActual[100];
            snprintf(idPartidaActual, sizeof(idPartidaActual), "%s", row[0]); // Convertir el ID a cadena
            char adminID[100];
            snprintf(adminID, sizeof(adminID), "%s", row[1]); // ID Admin
            char fechaPartidaActual[100];
            snprintf(fechaPartidaActual, sizeof(fechaPartidaActual), "%s", row[2]);  // Fecha del evento

            snprintf(textoResultado + strlen(textoResultado), sizeof(textoResultado) - strlen(textoResultado), "%s", idPartidaActual);

            // OBTENER NOMBRE DEL ADMIN
            char query2[512];
            snprintf(query2, sizeof(query2), 
                            "SELECT Username FROM Users WHERE Player_ID = %d", atoi(adminID));
			
			MYSQL_RES *result2;  // No sobrescribir el primer 'result'
			MYSQL_ROW row2;

            // Ejecutar la consulta
            if (mysql_query(conn, query2)) {
                fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
                return;
            }
			
			// Obtener los resultados de la segunda consulta
			result2 = mysql_store_result(conn);
			if (result2 == NULL) {
				fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
				return;
			}
			
			row2 = mysql_fetch_row(result2);
			if (row2 != NULL) {
				char adminName[100];
				snprintf(adminName, sizeof(adminName), "%s", row2[0]);
				snprintf(textoResultado + strlen(textoResultado), sizeof(textoResultado) - strlen(textoResultado), "=%s", adminName);
			}
			
			snprintf(textoResultado + strlen(textoResultado), sizeof(textoResultado) - strlen(textoResultado), "=%s", fechaPartidaActual);
			
			// Liberar los resultados de la segunda consulta
			mysql_free_result(result2);
		}
		
		printf("%s\n", textoResultado);
		
		strcpy(response, textoResultado);
	}
	
	// Liberar el resultado de la primera consulta
	mysql_free_result(result);
}


// FUNCTION TO INITIALIZE THE CONNECTION WITH MYSQL
MYSQL* init_mysql_connection() {
	MYSQL* conn = mysql_init(NULL);
	if (conn == NULL)
    {
		printf("Error creating the connection with MySQL: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	conn = mysql_real_connect(conn,"shiva2.upc.es", "root", "mysql", "T5_BBDDJuego", 0, NULL, 0);
	if (conn == NULL)
    {
		printf("Error initializing the connection with MySQL: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	return conn;
}


// SEND CHAT MESSAGE
// |-----------------------------------------------------------------------------------------------------------| 
// | Function: broadcast_message_chat                                                                          | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Description: Broadcasts a chat message to all connected clients except the sender. Ensures thread safety  | 
// | by locking a mutex during the broadcasting process.                                                       | 
// |-----------------------------------------------------------------------------------------------------------| 
// | Input: | // | - char* chat_message: The message to be broadcasted.                                        | 
// | - char* username: The username of the sender.                                                             | 
// | - char* response: A buffer to format the broadcast message.                                               | 
// | - int sender_sock: The socket descriptor of the sender (to exclude from broadcasting).                    | 
// | Output: Sends the formatted chat message to all connected clients except the sender.                      | 
// |-----------------------------------------------------------------------------------------------------------|
void broadcast_message_chat(char* chat_message, char* username, char* response,int sender_sock) {
	pthread_mutex_lock(&mutex); // Bloqueo para evitar problemas de concurrencia
	sprintf(response, "101/%s:%s\n",username,chat_message); 
	for (int j= 0;j<client_count; j++) {
		
		if (sockets[j] != NULL){
			if (sockets[j]->sock_conn != sender_sock) 
				write(sockets[j]->sock_conn, response, strlen(response)); // Enviar el mensaje a cada cliente
		}
	}
	pthread_mutex_unlock(&mutex); // Desbloqueo
}
	
// MAIN
int main() {
	int sock_listen, sock_conn;
	struct sockaddr_in serv_adr;
	MYSQL* conn;
	pthread_t threads[MAX_CLIENTS]; //Array of threads
	//Number of connected clients
	
	// conection with MYSQL
	conn = init_mysql_connection();
	
	// create socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
    {
		printf("Error connecting to socket\n");
		exit(1);
	}
	
	// configuration of the server
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(PORT);
	
	// asign socket to port
	if (bind(sock_listen, (struct sockaddr*)&serv_adr, sizeof(serv_adr)) < 0)
    {
		printf("Error doing the bind\n");
		exit(1);
	}
	
	// Listen connections
	if (listen(sock_listen, MAX_CLIENTS) < 0)
    {
		printf("Error in listen\n");
		exit(1);
	}
	
	printf("=======================================\n");
	printf("========== T05 - PROYECTO SO ==========\n");
	printf("========== SERVIDOR ACTIVADO ==========\n");
	printf("=======================================\n");

	
	// loop to habdle connections
	while (1)
    {
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Connection recieved\n");
		sockets[client_count] = sock_conn;
		
		//structure to pass the parameters to the thread
		struct thread_data* t_data = (struct thread_data*)malloc(sizeof(struct thread_data));
		t_data->sock_conn = sock_conn;
		t_data->conn = conn;
		strcpy(t_data->username, "");
		
		sockets[client_count] = t_data;
		
		//thread to manage the client
		pthread_create(&threads[client_count], NULL, handle_client, (void*)t_data);
		
		client_count++;
		
		//limit the number of connected clients
		if (client_count >= MAX_CLIENTS) {
			client_count = 0;
		}
	}
	
	mysql_close(conn);
	return 0;
 }
