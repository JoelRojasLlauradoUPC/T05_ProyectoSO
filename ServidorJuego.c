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

//INTERACCION
void register_user(char* username, char* password, char* response, MYSQL* conn);
void login_user(char* username, char* password, char* response, MYSQL* conn);

//FUNCTION TO ADD PLAYER TO THE CONNECTED LIST
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
		
		for (int i = 0; i < player_count; i++) //Generate the vector
		{
			strcat(response, connected_players[i]);
			strcat(response, "-");
			printf("Respuesta: ");
			printf(response);
			printf("\n");
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

		case 300: //Dado tirado
			p = strtok(NULL, "/");
			printf(p);
			printf("\n");
			if (p != NULL)
			{
				moveToken(p, response, conn);
			}
			printf("FINAL 300\n");

			break;
		
		case 0:
			p = strtok(NULL, "/");
			if (p != NULL) {
				remove_player(p);
				strcpy(response, "End of connection\n");
			}
			else {
				strcpy(response, "Player not specified for disconecction\n");
			}
            break;
		default:
            strcpy(response, "Invalid code\n");
			break;
    }
}

// CLIENT
void* handle_client(void* data) {
	struct thread_data* t_data = (struct thread_data*)data;
	int sock_conn = t_data->sock_conn;
	MYSQL* conn = t_data->conn;
	
	char request[512];
	char response[512];
	int ret;
	int terminar = 0;
	
	while (!terminar) {
		ret = read(sock_conn, request, sizeof(request) -1);
		request[ret] = '\0';

		
		printf("Received request: %s\n", request);
		
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

// REGISTER A NEW USER
void register_user(char* username, char* password, char* response, MYSQL* conn) {
	char query[512];
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	
	// check if the username already exists
	snprintf(query, sizeof(query),
			 "SELECT Username FROM Users WHERE Username='%s'", username);
	
	if (mysql_query(conn, query) != 0) {
		snprintf(response, 512, "Error verifying user: %u %s\n",
				 mysql_errno(conn), mysql_error(conn));
		return;
	}
	resultado = mysql_store_result(conn);
	if (resultado == NULL) {
		snprintf(response, 512, "Error obtaining result: %u %s\n",
				 mysql_errno(conn), mysql_error(conn));
		return;
	}
	
	if (mysql_num_rows(resultado) > 0) {
		strcpy(response, "This user already exists.\n");
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
		snprintf(response, 512, "Error registering: %u %s\n",
				 mysql_errno(conn), mysql_error(conn));
	}
}
	
// LOGIN AN EXISTING USER
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
    snprintf(query1, sizeof(query1), "INSERT INTO Game (GameStatus, PosicionFichas, GameAdmin) VALUES (2, '0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0', %d);", player_id);

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
			printf("PONGO IDJP\n");
			sockets[currentSearchingSocket]->IdentificadorPartida = max_game_id;
			encontrado = true;
			printf("PONGO IP\n");
		}
		currentSearchingSocket++;
	}


	
	

	printf("\n");
	
	printf("ENVIADO 201\n");
	printf(response);
	printf("\n");
}

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
    while (currentSearchingSocket < player_count && !socketEncontrado) {
        if (strcmp(sockets[currentSearchingSocket]->username, jugadorinvitado) == 0) {
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
            } else {
                printf("Error: Socket inválido para el jugador %s.\n", jugadorinvitado);
            }
        }
        currentSearchingSocket++;
    }

    if (!socketEncontrado) {
        printf("Jugador %s no encontrado.\n", jugadorinvitado);
    }
}

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
			printf("PONGO IDJP\n");
			sockets[currentSearchingSocket]->IdentificadorPartida = game_id;
			encontrado = true;
			printf("PONGO IP\n");
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


void readyToStartGame(const char* input, char* response, MYSQL* conn, int SenderSockID)
{
	printf("GAME ");
	printf(input);
	printf(" REQUESTED TO START\n");

	// Insertar el registro en la tabla Game_Members
    char query0[400];
    snprintf(query0, sizeof(query0), 
             "UPDATE Game SET GameStatus = %d WHERE Game_ID = %d;", 
             1, atoi(input));

    // Ejecutar la consulta INSERT en Game_Members
    if (mysql_query(conn, query0)) {
        fprintf(stderr, "Error en la consulta INSERT: %s\n", mysql_error(conn));
        return;
    }

    printf("Estado de partida actualizado a 1\n");

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


void moveToken(const char* input, char* response, MYSQL* conn)
{
    char currentGameID[100];
    char jugadorInteractuando[100];  // Variable para guardar el segundo token
    int numeroAleatorio;

    // Usamos strtok para separar la cadena
    char *token = strtok(input, "-");
    strcpy(currentGameID, token);  // El primer token es el currentGameID

    // El segundo token es el jugadorInteractuando
    token = strtok(NULL, "-");
    strcpy(jugadorInteractuando, token);  // Guardamos el segundo token

    // El tercer token es el numeroAleatorio
    token = strtok(NULL, "-");
    numeroAleatorio = atoi(token);  // Convertimos el tercer token a entero

    // Imprimir los resultados
    printf("currentGameID: %s\n", currentGameID);
    printf("jugadorInteractuando: %s\n", jugadorInteractuando);
    printf("numeroAleatorio: %d\n", numeroAleatorio);

	char query0[256];
    MYSQL_RES *result;
    MYSQL_ROW row;

    // Construir la consulta SQL
    snprintf(query0, sizeof(query0), "SELECT Game.PosicionFichas FROM Game WHERE Game.Game_ID = %d;", atoi(currentGameID));

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
        printf("PosicionFichas para Game_ID %d: %s\n", atoi(currentGameID), row[0]);
    } else {
        printf("No se encontró ningún resultado para Game_ID %d\n", atoi(currentGameID));
    }


	// Crear una copia de la cadena original para trabajar
    char temp[254];
    strcpy(temp, row[0]);

    // Crear un puntero para el token
    char *tokenS = strtok(temp, "-");

    // Convertir el primer token (número) a entero
    int pos0 = atoi(tokenS);

    // Sumar el valor aleatorio
    pos0 += numeroAleatorio;

    // Convertir el número modificado de nuevo a cadena
    char num_str[10];
    sprintf(num_str, "%d", pos0);

    // Construir la nueva cadena
    char nuevaCadena[20];
    strcpy(nuevaCadena, num_str); // Copiar el nuevo número

    // Agregar los tokens restantes si es necesario
    tokenS = strtok(NULL, ""); // Tomar el resto de la cadena original
    if (tokenS != NULL) {
        strcat(nuevaCadena, "-");
        strcat(nuevaCadena, tokenS);
    }

    // Imprimir la cadena resultante
    printf("Cadena modificada: %s\n", nuevaCadena);

	char newPosicionFichas[256];



	// Prepara la consulta SQL con el nuevo valor de PosicionFichas y el Game_ID
    char query1[512];
    snprintf(query1, sizeof(query1), 
        "UPDATE Game SET PosicionFichas = '%s' WHERE Game_ID = %d;", 
        nuevaCadena, atoi(currentGameID));

    // Ejecutar la consulta
    if (mysql_query(conn, query1)) {
        fprintf(stderr, "Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return;
    }



    // Mostrar la consulta generada (esto es solo para ver cómo quedó la consulta)
    printf("Consulta SQL: %s\n", query1);
	sendPositionsToGame(atoi(currentGameID), response, conn);
    printf("ENVIADO");

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

//chat send
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
