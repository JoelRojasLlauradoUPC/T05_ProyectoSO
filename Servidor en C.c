#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

//------------------------------------------------------------------
// Estructura para el acceso excluyente
//------------------------------------------------------------------

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

// pthread_mutex_lock( &mutex );
// No me interumpas

// Operaciones (estructuras de datos compartidas por threads
// o los threads modifican esas estructuras de datos
// ejemplo del contador

// pthread_mutex_unlock( &mutex);
// Ya puedes interrumpirme

//------------------------------------------------------------------
//------------------------------------------------------------------

int i;
int sockets[100];


typedef struct {
	char nombre [20];
	int socket;
} Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;

ListaConectados miLista;

int Pon (ListaConectados *lista, char nombre[20], int socket) {
	// Añade nuevo conectados. retorna 0 si OK y -1 si la lista ya estaba llena y no lo ha podido añadir
	if (lista->num == 100)
		return -1;
	else {
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		return 0;
	}
}

int DamePosicion (ListaConectados *lista, char nombre[20]) {
	// Devuelve la posición en la lista o -1 si no está en la lista
	int i = 0;
	int encontrado = 0;
	while ((i < lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
			encontrado = 1;
		if (!encontrado)
			i = i + 1;
	}
	if (encontrado)
		return i;
	else
		return -1;
}

int Elimina (ListaConectados *lista, char nombre[20]) {
	// Retorna 0 si elimina y -1 si el usuario no está en la lista
	int pos = DamePosicion (lista, nombre);
	if (pos == -1)
		return -1;
	else {
		int i;
		for (i=pos; i < lista->num-1; i++)
		{
			lista->conectados[i] = lista->conectados[i+1];
			// strcpy (lista->conectados[i].nombre, lista->conectados[i+1].nombre);
			// lista->conectados[i].socket = lista->conectados[i+1].socket;
		}
		lista->num--;
		return 0;
	}
}

void DameConectados(ListaConectados *lista, char conectados[300]) {
	sprintf(conectados, "%d", lista->num);
	for (int i = 0; i < lista->num; i++) {
		sprintf(conectados, "%s/%s", conectados, lista->conectados[i].nombre);
	}
}


void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	// int socket_conn = * (int *) socket;
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	MYSQL *conn;
	int err;
	
	// Create a connection to the MYSQL server
	conn = mysql_init(NULL);
	if (conn == NULL) {
		printf ("Error creating connection: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	// Initialize the connection, entering our access keys and the database name
	conn = mysql_real_connect (conn, "localhost", "root", "mysql", "ProyectoSO", 0, NULL, 0);
	if (conn == NULL) {
		printf ("Error initializing the connection: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	int terminar =0;
	// Entramos en un bucle para atender todas las peticiones de este cliente hasta que se desconecte
	while (terminar ==0)
	{
		// Ahora recibimos la petición
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que añadirle la marca de fin de string para que no escriba lo que hay despues del buffer
		peticion[ret]='\0';
		
		printf("Peticion: %s\n", peticion);
		
		// Vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo = atoi (p);
		// Ya tenemos el codigo de la peticion
		char nombre[25];
		char password[25];
		
		
		if (codigo !=0)
		{
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			// Ya tenemos el nombre
			printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
		}
		if (codigo == 0) {
			terminar = 1;
			Elimina(&miLista, nombre);
		} 
		
		else if (codigo == 1) {  // Register  --> 1/Username/Password
			p = strtok(NULL, "/");
			strcpy(password, p);
			printf("Password: %s\n", password);
			
			// Check if the user already exists
			char query[256];
			sprintf(query, "SELECT Username FROM Users WHERE Username='%s'", nombre);
			err = mysql_query(conn, query);
			
			if (err != 0) {
				printf ("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			MYSQL_RES *result = mysql_store_result(conn);
			int num_rows = mysql_num_rows(result);
			mysql_free_result(result);
			
			if (num_rows > 0) {
				sprintf(respuesta, "0");  // Username already exists
			} else {
				// Insert the new user into the database
				sprintf(query, "INSERT INTO Users (Username, Password) VALUES ('%s', '%s')", nombre, password);
				err = mysql_query(conn, query);
				
				if (err != 0) {
					printf ("Error inserting data into database: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				} else {
					sprintf(respuesta, "1");  // User registered successfully
					
				}

			}
			
		} 
		
		else if (codigo == 2) {  // Log in --> 2/Username/Password
			p = strtok(NULL, "/");
			strcpy(password, p);
			
			// Check if the user exists and password matches
			char query[256];
			sprintf(query, "SELECT Username, Password FROM Users WHERE Username='%s' AND Password='%s'", nombre, password);
			err = mysql_query(conn, query);
			
			if (err != 0) {
				printf ("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			MYSQL_RES *result = mysql_store_result(conn);
			int num_rows = mysql_num_rows(result);
			mysql_free_result(result);
			
			if (num_rows == 0) {
				sprintf(respuesta, "0");  // Login failed
			} else {
				sprintf(respuesta, "1");  // Login successful
				Pon(&miLista, nombre, sock_conn);
			}
		}
		
		else if (codigo == 3) { // Select players of a game --> 3/Player_ID/Game
			int game_id = atoi(p);
			
			// Preparar la consulta para seleccionar
			char select_query[256];
			snprintf(select_query, sizeof(select_query), "SELECT DISTINCT Users.Username FROM Users JOIN Game_Record ON Users.Player_ID = Game_Record.Player_ID WHERE Game_Record.Game_ID = %d", game_id);
			
			// Ejecutar la consulta de selecci n
			int err = mysql_query(conn, select_query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Almacenar resultados
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Procesar resultados
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Inicializa la cadena de respuesta
			while ((row = mysql_fetch_row(result)) != NULL) {
				if (row[0] != NULL) {
					// A adir Player_ID a la respuesta
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1);
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1);
				}
			}
			
			// Liberar resultados
			mysql_free_result(result);
			
			// Imprimir la respuesta final
			printf("Player IDs for Game ID %d:\n%s", game_id, respuesta);
			
		}
		
		else if (codigo == 4) { // See points of a game --> 4/Game
			int game_id = atoi(p);
			
			// Preparar la consulta
			char query[256];
			snprintf(query, sizeof(query), "SELECT Points FROM Game_Record WHERE Game_ID = %d", game_id);
			
			// Ejecutar la consulta
			int err = mysql_query(conn, query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Almacenar resultados
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Procesar resultados
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Inicializa la cadena de respuesta
			while ((row = mysql_fetch_row(result)) != NULL) {
				// Verificar que row[0] no sea NULL
				if (row[0] != NULL) {
					// A adir los puntos a la respuesta
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1); // A adir puntos
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1); // A adir nueva l nea
				}
			}
			
			// Liberar resultados
			mysql_free_result(result);
			
			// Imprimir la respuesta final
			printf("Points:\n%s", respuesta);
		}
		
		else if (codigo == 5) { // See games played by a player --> 5/Player
			int player_id = atoi(p);
			
			// Preparar la consulta
			char query[256];
			snprintf(query, sizeof(query), "SELECT Game_ID FROM Game_Record WHERE Player_ID = %d", player_id);
			
			// Ejecutar la consulta
			int err = mysql_query(conn, query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Almacenar resultados
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Procesar resultados
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Inicializa la cadena de respuesta
			while ((row = mysql_fetch_row(result)) != NULL) {
				// Verificar que row[0] no sea NULL
				if (row[0] != NULL) {
					// A adir los puntos a la respuesta
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1); // A adir puntos
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1); // A adir nueva l nea
				}
			}			
			
			// Liberar resultados
			mysql_free_result(result);
			
			// Imprimir la respuesta final
			printf("Played Games:\n%s", respuesta);
		}
		
		else if (codigo == 7) {
			char misConectados[512];
			DameConectados(&miLista, misConectados);
			strcpy(respuesta, misConectados);
			printf("%s\n", respuesta);
		}
		
		if (codigo != 0) {
			// Send response to client
			write(sock_conn, respuesta, strlen(respuesta));
		}
	}
	
	// Close connection for this client
	close(sock_conn);
}


int main(int argc, char *argv[])
{
    // C  reate the socket
    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;
    

    // INITIALIZATIONS
    // Open the socket
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
        printf("Error creating socket");
    
    // Bind to the port
    memset(&serv_adr, 0, sizeof(serv_adr));  // Initialize serv_addr to zero
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);  // Listen on any IP address
    serv_adr.sin_port = htons(9060);  // Listen on port 9080
    
    if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
        printf ("Error on bind");

    // The queue of pending requests cannot exceed 4
    if (listen(sock_listen, 4) < 0)
        printf("Error on listen");	
	
	i = 0;
	pthread_t thread;
    
    for (;;) {
        printf ("Listening\n");

        sock_conn = accept(sock_listen, NULL, NULL);
        printf ("Received a connection\n");
		
		sockets[i] = sock_conn;
		
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
						
		i = i+1;
           
    }
}
