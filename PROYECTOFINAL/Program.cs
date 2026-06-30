using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace PROYECTOFINAL
{
    internal class Program
    {
        //VARIABLES GLOBALES
        static List<string> productosGlobal = new List<string>();
        static int totalVentas = 0;
        static double gananciasDia = 0;
        static double totalProductosVendidos = 0;
        static Dictionary<string, double> productosVendidos = new Dictionary<string, double>();
        static string archivoDeudores = "DEUDORES.txt";
        //LECTURA DEL ARCHIVO
        static string[] LEERARCHIVO()
        {
            return File.ReadAllLines("BASEDATOS.txt");
        }
        // NORMALIZACION
        public static string QUITARTILDES(string texto)
        {
            string normalizado = texto.Normalize(NormalizationForm.FormD);
            StringBuilder resultado = new StringBuilder();
            foreach (char letra in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra)
                    != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(letra);
                }
            }
            return resultado.ToString().Normalize(NormalizationForm.FormC);
        }
        //SALIR
        public static bool SALIR(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto) &&
          texto.Trim().ToUpper() == "SALIR"; 
        }

        //SESION
        public static void REINICIODATOS()
        {
            //REINICIAR DATOS A CERO
        }
        public static void CARGARSESION()
        {
            //CARGAR SESIÓN
            //CREANDO ARCHIVOS TXT
        }
        public static void CARGARVENTASDIA()
        {

        }

        //VENTAS
        public static void REGISTROVENTAS()
        {
            string[] linea = LEERARCHIVO();
            //VARIABLES DE CONTROL 
            string IMPUT2;
            bool buscarOtro = false;
            //VARIABLES NORMALES
            (bool exito, double importe, string historial) RESULT = default;
            double SUBTOTAL = 0;
            string historialTotal = "";
            do
            {
                //VARIABLE TEMPORAL
                int contador = 1;
                bool BUSCADOR = false;
                Console.WriteLine("========== REGISTRAR VENTA ==========");
                Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");
                Console.WriteLine("INGRESE EL NOMBRE DEL PRODUCTO: ");
                string nomProd = Console.ReadLine();
                if (SALIR(nomProd))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
                //Lista de posiciones de productos
                List<int> posiciones = new List<int>();
                for (int x = 0; x < linea.Length; x++)
                {
                    if (linea[x].StartsWith("====") || linea[x].StartsWith("ID") || linea[x].StartsWith("---"))
                    {
                        continue;
                    }
                    // Cada linea en un arreglo
                    string[] DATO = linea[x].Split('|');
                    //Verificamos que la linea tiene 7 arreglos
                    if (DATO.Length < 7)
                    {
                        continue;
                    }
                    //Limpiamos los espacios
                    for(int i =0; i < DATO.Length; i++)
                    {
                        DATO[i] = DATO[i].Trim();
                    }
                    //Normalizamos nombre
                    if (QUITARTILDES(DATO[1]).ToUpper().Contains(QUITARTILDES(nomProd).ToUpper()))
                    {
                        BUSCADOR = true;
                        posiciones.Add(x);
                        Console.WriteLine(contador + ". " + DATO[1] + "|" + DATO[2] + "|" + DATO[3] + "|" + DATO[5] + "|" + DATO[6]);
                        contador++;
                    }
                }
                if (BUSCADOR)
                {
                    //VARIABLES DE CONTROL 
                    bool ACEPPROD = false;
                    //VARIABLES 
                    int INDICE;
                    string [] Dato = null;
                    while (!ACEPPROD)
                    {
                        int Opc;
                        int Opc2;
                        do
                        {
                            Console.WriteLine("========================");
                            Console.Write("SELECCIONE EL PRODUCTO: ");
                            string SELEC = Console.ReadLine();
                            if (SALIR(SELEC))
                            {
                                Console.WriteLine("Operación cancelada.");
                                return;
                            }
                            if (!int.TryParse(SELEC, out Opc) || Opc < 1 || Opc > posiciones.Count)
                            {
                                Console.WriteLine("Entrada no valida, Ingrese un número de la lista");
                            }
                        } while (Opc < 1 || Opc > posiciones.Count);
                        //RELACION INDICE - ENTRADA
                        INDICE = posiciones[Opc - 1];
                        //SEPARACION DE LINEA EN ARREGLOS
                        Dato = linea[INDICE].Split('|');
                        for(int i =0; i<Dato.Length; i++)
                        {
                            Dato[i] = Dato[i].Trim();
                        }
                        //MOSTRANDO PRODUCTO
                        Console.WriteLine("=========================");
                        Console.WriteLine("  PRODUCTO SELECCIONADO  ");
                        Console.WriteLine(Dato[1] +" | "+ Dato[2] + " | " + Dato[3] + " | " + Dato[5] + " | " + Dato[6]);
                        Console.WriteLine("=========================");
                        //MENU DE CONFIRMACION
                        Console.WriteLine("1. Continuar");
                        Console.WriteLine("2. Elegir otro del listado");
                        Console.WriteLine("3. Buscar otro producto");
                        do
                        {
                            Console.Write("SELECIONE UN OPCION: ");
                            string SELECT2 = Console.ReadLine();

                            if (SELECT2.ToUpper() == "SALIR")
                                return;

                            if (!int.TryParse(SELECT2, out Opc2) || Opc2<1 || Opc2 >3)
                            {
                                Console.WriteLine("Entrada Invalida, Ingrese un número del rango (1-3)");
                                continue;
                            }
                        } while (Opc2 < 1 || Opc2 > 3);
                        //ACCION SEGUN EL NUMERO DE ELECCION
                        switch (Opc2)
                        {
                            case 1:
                                ACEPPROD = true;
                                break;
                            case 2:
                                //VARIABLE DE CORRELATIVO
                                int COR=1;
                                Console.WriteLine("========================");
                                Console.WriteLine("    LISTA DE PRODUCTO   ");
                                Console.WriteLine("========================");
                                //VUELVO A RECORRER LAS POSICIONES GUARDADAS
                                foreach(int D in posiciones)
                                {
                                    string[] Lista = linea[D].Split('|');
                                    for(int i = 0; i<Lista.Length; i++)
                                    {
                                        Lista[i] = Lista[i].Trim();
                                    }
                                    Console.WriteLine(COR + " | " + Lista[1] + " | " + Lista[2] + " | " + Lista[3] + " | " + Lista[5] + " | " + Lista[6]);
                                    COR++;
                                }
                                Console.WriteLine("========================");
                                break;
                            case 3:
                                buscarOtro = true;
                                ACEPPROD = true;
                                break;
                        }
                        //PIDO LA CANTIDAD
                        //VARIABLE DE RETORNO
                        double CANTIDAD = CANTIDADVENTA(Dato, ref linea, INDICE);
                        if (CANTIDAD == -1)
                        {
                            return;
                        }
                        //CALCULO EL STOCK
                        //VARIABLE DE RETORNO Y CÁLCULO
                        RESULT = CONTROL_STOCK(Dato, ref linea, INDICE, CANTIDAD);

                        if (RESULT.exito)
                        {
                            SUBTOTAL += RESULT.importe;
                            historialTotal += RESULT.historial;
                        }
                    }
                }

                if (!BUSCADOR)
                {
                    Console.WriteLine("Producto no encontrado....");
                }
                //VARIABLE DE CONFIRMACION
                Console.Write("DESEA INGRESAR OTRO PRODUCTO: S/N");
                IMPUT2 = Console.ReadLine().ToUpper().Trim(); ;
                if (SALIR(IMPUT2))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
                while (string.IsNullOrWhiteSpace(IMPUT2) ||!(IMPUT2 == "SI" || IMPUT2 == "S" || IMPUT2 == "NO" || IMPUT2 == "N"))
                {
                    Console.WriteLine("Ingrese Si o No:");
                    IMPUT2 = Console.ReadLine().ToUpper();
                    if (SALIR(IMPUT2))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }
                }
                if (buscarOtro)
                {
                    continue;
                }

            } while (IMPUT2 == "SI" || IMPUT2 == "S");
            if(SUBTOTAL <= 0)
            {
                Console.WriteLine("No se registró ninguna venta.");
                return;
            }
            Console.WriteLine("=============================");
            Console.Write("  ¿CONFIRMA LA COMPRA? (S/N): " );
            string CF = Console.ReadLine().ToUpper().Trim() ;
            if (SALIR(CF))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            while (string.IsNullOrWhiteSpace(CF) ||!(CF == "SI" || CF == "S" || CF == "NO" || CF == "N"))
            {
                Console.WriteLine("Ingrese Si o no...");
                CF = Console.ReadLine().ToUpper().Trim();
                if (SALIR(CF))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }
            if (CF == "NO" || CF == "N")
            {
                Console.WriteLine("Venta cancelada.");
                return;
            }
            METODOPAGO(SUBTOTAL, historialTotal);
            File.WriteAllLines("BASEDATOS.txt", linea);
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("=============================");


        }
        public static double CANTIDADVENTA(string[] Dato, ref string[] linea, int indice)
        {
            //PASO POR REFERENCIA A DATO PARA LA SEPARACION DE LINEAS EN ARREGLOS
            //PASO POR REFERENCIA LINEA PARA LA LECTURA DEL ARCHIVO
            //PASO POR REFERENCIA INDICE PARA LAS POSICIONES DONDE ESTÁ LOS PRODUCTOS

            //VARIABLES NORMALES
            double CANTS=0;
            int CANTSE;
            //VARIABLES DE CONTROL
            bool valido = true;
            do
            {
                Console.WriteLine("==================");
                Console.Write("INGRESE LA CANTIDAD: ");
                string IMPUT = Console.ReadLine();
                if (SALIR(IMPUT))
                {
                    Console.WriteLine("Operación cancelada.");
                    return -1;
                }
                if (Dato[5].ToUpper().Contains("KG"))
                {
                    valido = double.TryParse(IMPUT, NumberStyles.Any, new CultureInfo("es-PE"), out CANTS);
                    if (!valido || CANTS <= 0)
                    {
                        Console.WriteLine("Cantidad invalida....");
                    }
                }
                else
                {
                    valido = int.TryParse(IMPUT, out CANTSE);
                    CANTS = CANTSE;
                    if (!valido || CANTSE <= 0)
                    {
                        Console.WriteLine("Cantidad invalida...Ingrese valores enteros. ");
                    }
                }
            } while (!valido || CANTS <= 0);
            return CANTS;
        }
        public static (bool exito,double importe, string historial) CONTROL_STOCK(string[] Dato, ref string[] linea, int indice, double Cantidad)
        {
            //VARIABLE NORMALES
            double STOCK = double.Parse(Dato[4].Replace(".", ",").Trim(), CultureInfo.InvariantCulture);
            double PRECIO = double.Parse(Dato[6].Replace("S/", "").Trim(), CultureInfo.InvariantCulture);
            string UNIDAD = Dato[5];
            bool HVENTA = false;
            double IMPORTE=0;
            string HISTORIAl = "";
            string CLAVE = Dato[1] + " (" + UNIDAD + ")"; // PARA GUARDAR EN EL DICCIONARIO
            if (Cantidad <= STOCK)
            {
                Console.WriteLine("Stock Disponible...");
                STOCK = STOCK - Cantidad;
                HVENTA = true;
                Dato[4] = STOCK.ToString();
                //REEMPLAZAMOS LA LINEA SIN MODIFICAR
                linea[indice] = string.Join("|", Dato);
                //CALCULO DEL IMPORTE
                IMPORTE = Cantidad * PRECIO;
                HISTORIAl += "\nPRODUCTO: " + Dato[1] + " | " + Dato[2] + " | " + Dato[3] +
                            "\nCANTIDAD: " + Cantidad +
                            "\nPRECIO: " + PRECIO +
                            "\nIMPORTE: " + IMPORTE;

                productosGlobal.Add(Dato[1]);
                totalProductosVendidos += Cantidad;
                totalVentas++;
                if (productosVendidos.ContainsKey(CLAVE))
                {
                    productosVendidos[CLAVE] += Cantidad;
                }
                else
                {
                    productosVendidos.Add(CLAVE, Cantidad);
                }
            }
            else
            {
                Console.WriteLine("Stock insuficiente...");
            }
            return (HVENTA, IMPORTE, HISTORIAl);
        }
        public static void METODOPAGO() 
        {
            //DEBE TENER 4 METODOS DE PAGO: YAPE, TRANFERENCIA, EFECTIVO,CREDITO(NORMAL Y FIADO(MAX 7), TARJETA(+4%). 
            //DEBE SER TENER LA CAPACIDAD DE PAGAR MIXTO(USANDO MINIMO 2 METODO DIFERENTES DE PAGO) 
            //EFECTIVO DEBE TENER LA CAPACIDAD QUE SI DESEA PAGAR O DAR UN ADELANTO DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS
            //LOS ADELANTOS SE DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS.
            //CREDITO TAMBIEN DEBE GENERARA Y GUARDAR LA INFORMACIÓN EN EL MISMO TXT DE DEUDAS.
            //A CREDITO SE LE DEBEN PONER DÍAS DE INICIO Y VENCIMIENTO
            //SOLO SI EL PAGO ESTÁ COMPLETO O EL METODO DE PAGO ES TARJETA O ES YAPE/TRANSFERENCIA, SE LE DEBE LLAMAR A LA OPCION DE SI DESEA UN TIPO COMPROBANTE
            // SE DEBE GUARDAR UN HISTORIALES
        }
        public static void TIPOCOMPROBANTE()
        {
            // DAR 2 OPCIONES DE COMPROBANTE: BOLETA O FACTURA
            // CREAR 2 ARCHIVOS DIFERENTES PARA BOLETAS Y PARA FACTURAS POR DÍA
            //CADA FACTURA Y BOLETA DEBE TENER UN CORRELATIVO Y SU NUMERO DE SERIE.
            //PARA BOLETA DEBE DAR LA OPCION DE 3 TIPOS DE DOCUMENTO( DNI, CARNET EXTR, SIN DOCUMENTO
            //

        } 
        public static void DELIVERY()
        {
            //SE DEBE TENER LAS PAUTAS DE DELIVERY
            //SE DEBE CALCULAR ANTES DE DAR EL RESUMEN DE COMPRA COMPLETO, AUNQUE SE DEBE MOSTRAR ANTES DE PEDIR EL DELIVERY EL MONTO FINAL SOLO
            //DEBE ESTAR RELACIONADO CON OTROS METODOS
        }
        public static int NUMBOLETA()
        {
            string archivo = "BASEDATOS.txt";
            if (!File.Exists(archivo))
            {
                File.WriteAllText(archivo, "0");
            }
            int contador = int.Parse(File.ReadAllText(archivo));
            contador++;
            File.WriteAllText(archivo, contador.ToString());
            return contador;
        }
        public static int NUMFACTURA()
        {
            string archivo = "contador_factura.txt";
            if (!File.Exists(archivo))
            {
                File.WriteAllText(archivo, "0");
            }
            int contador = int.Parse(File.ReadAllText(archivo));
            contador++;
            File.WriteAllText(archivo, contador.ToString());
            return contador;
        }
        public static void HISTORIALVENTA()
        {

        }

        // ESTADISTICA DE VENTAS
        public static void ESTADISTICAS()
        {
            //CREACIÓN DE ESTADITICAS
        }
        public static void CIERRECAJA()
        {
            //CREACIÓN DE CIERRE DE CAJA, CON ARCHIVO TXT
        }
        
        //PROVEEDORES
        public static void OBTENERIDPROV()
        {

        }
        public static void VERIFICAR()
        {
            //VERIFICAR CREACIÓN CORRECTA DE ARCHIVOS
        }
        public static void MENUPROV()
        {

        }
        public static void MOSTRARPROV()
        {

        }
        public static void AGREGARPROV()
        {

        }
        public static void MODIFICARPROV()
        {

        }

        //REGISTRAR COMPRAS
        public static void AGREGARPROD()
        {

        }
        public static void BUSCARPROD()
        {

        }
        public static void MODIFICARPROD()
        {

        }
        public static void ELIMINARPRODUCTO()
        {

        }
        public static void MENUPROD()
        {

        }

        // DEUDAS
        public static void DEUDASVENCIDAS(string archivoDeudores) // archivo: ruta del archivo de deudores
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores); // Lee todas las líneas del archivo

            // Variables para almacenar datos del bloque actual
            string cliente = "";
            decimal saldo = 0;
            int diasCredito = 0;
            DateTime fechaInicio = DateTime.MinValue;

            bool bloque = false;    // Indica si estamos dentro de un bloque de cliente
            bool tieneDatos = false; // Indica si ya se encontró SALDO RESTANTE

            Console.WriteLine("=== DEUDAS VENCIDAS ===");

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    // Inicio de un nuevo bloque de cliente
                    cliente = linea.Substring("CLIENTE:".Length).Trim();
                    bloque = true;
                    tieneDatos = false;
                }
                else if (bloque && linea.StartsWith("SALDO RESTANTE:"))
                {
                    // Prioridad: usa SALDO RESTANTE si existe (abonos previos)
                    decimal.TryParse(linea.Substring("SALDO RESTANTE:".Length).Trim(), out saldo);
                    tieneDatos = true;
                }
                else if (bloque && linea.StartsWith("TOTAL FINAL:") && !tieneDatos)
                {
                    // Si no hay SALDO RESTANTE, usa el TOTAL FINAL como saldo
                    decimal.TryParse(linea.Substring("TOTAL FINAL:".Length).Trim(), out saldo);
                }
                else if (bloque && linea.StartsWith("DIAS PARA PAGAR:"))
                {
                    // Días de crédito otorgados al cliente
                    int.TryParse(linea.Substring("DIAS PARA PAGAR:".Length).Trim(), out diasCredito);
                }
                else if (bloque && linea.StartsWith("FECHA DE INICIO:"))
                {
                    // Fecha desde la que corre el crédito
                    DateTime.TryParse(linea.Substring("FECHA DE INICIO:".Length).Trim(), out fechaInicio);
                }
                else if (linea.StartsWith("================"))
                {
                    // Fin del bloque: evalúa si está vencido
                    if (cliente != "" && saldo > 0 && fechaInicio != DateTime.MinValue)
                    {
                        DateTime fechaVencimiento = fechaInicio.AddDays(diasCredito); // Calcula vencimiento

                        if (DateTime.Now > fechaVencimiento) // Si ya pasó la fecha límite
                        {
                            int diasVencido = (DateTime.Now - fechaVencimiento).Days; // Días de retraso

                            Console.WriteLine("----------------------------");
                            Console.WriteLine("CLIENTE          : " + cliente);
                            Console.WriteLine("SALDO PENDIENTE  : S/ " + saldo);
                            Console.WriteLine("FECHA VENCIMIENTO: " + fechaVencimiento.ToString("dd/MM/yyyy"));
                            Console.WriteLine("DÍAS VENCIDO     : " + diasVencido + " día(s)");
                        }
                    }

                    // Reset para el siguiente bloque
                    cliente = "";
                    saldo = 0;
                    diasCredito = 0;
                    fechaInicio = DateTime.MinValue;
                    bloque = false;
                    tieneDatos = false;
                }
            }

            Console.WriteLine("============================");
        }
        public static void ABONARDEUDA(string archivoDeudores)
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");

            // === INGRESO DE CLIENTE ===
            Console.Write("INGRESE EL NOMBRE DEL CLIENTE: ");
            string clienteBuscar = Console.ReadLine();
            if (clienteBuscar.ToUpper() == "SALIR") return;

            // === INGRESO Y VALIDACIÓN DEL MONTO ===
            string inputMonto;
            double montoAbono;

            Console.Write("INGRESE EL MONTO A ABONAR: S/ ");
            inputMonto = Console.ReadLine();
            if (inputMonto.ToUpper() == "SALIR") return;

            // Solo acepta números positivos
            while (!double.TryParse(inputMonto, out montoAbono) || montoAbono <= 0)
            {
                Console.WriteLine("Monto inválido. Ingrese un valor mayor a 0:");
                inputMonto = Console.ReadLine();
                if (inputMonto.ToUpper() == "SALIR") return;
            }

            // === MÉTODO DE PAGO (método separado explícito) ===
            string metodoPago = METODOPAGO();
            if (metodoPago == null) return; // Si canceló en METODOPAGO, sale

            // === LECTURA Y PROCESO DEL ARCHIVO ===
            string[] lineas = File.ReadAllLines(archivoDeudores);
            List<string> nuevasLineas = new List<string>();  // Guardará las líneas finales
            List<string> bloqueTemporal = new List<string>(); // Líneas del bloque actual

            bool bloqueCliente = false;  // Estamos en el bloque del cliente buscado
            bool encontrado = false;     // Si se encontró el cliente
            bool tieneSaldo = false;     // Si el bloque tiene SALDO RESTANTE
            bool saldoCalculado = false; // Si ya se procesó el pago
            bool guardarBloque = true;   // Si el bloque se guarda (false = deuda cancelada)

            double saldoBase = 0;  // Saldo antes del abono
            double nuevoSaldo = 0; // Saldo después del abono
            double vuelto = 0;     // Vuelto si pagó de más

            string clienteActual = ""; // Nombre del cliente encontrado

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    // Inicio de bloque: detecta si es el cliente buscado
                    clienteActual = linea.Substring("CLIENTE:".Length).Trim();
                    bloqueCliente = clienteActual.Equals(clienteBuscar, StringComparison.OrdinalIgnoreCase);
                    bloqueTemporal.Clear();
                    bloqueTemporal.Add(linea);
                }
                else if (linea.StartsWith("SALDO RESTANTE:"))
                {
                    if (bloqueCliente)
                    {
                        // Toma el saldo restante de abonos anteriores
                        double.TryParse(linea.Substring("SALDO RESTANTE:".Length).Trim(), out saldoBase);
                        tieneSaldo = true;
                        // No se agrega aquí: se recalculará luego
                    }
                    else
                    {
                        bloqueTemporal.Add(linea); // Otro cliente: se conserva tal cual
                    }
                }
                else if (linea.StartsWith("TOTAL FINAL:"))
                {
                    bloqueTemporal.Add(linea);
                    if (bloqueCliente && !tieneSaldo)
                    {
                        // Si no había SALDO RESTANTE, usa TOTAL FINAL como base
                        double.TryParse(linea.Substring("TOTAL FINAL:".Length).Trim(), out saldoBase);
                        tieneSaldo = true;
                    }
                }
                else if (linea.StartsWith("================"))
                {
                    // Fin del bloque: aplica el abono
                    if (bloqueCliente && tieneSaldo && !saldoCalculado)
                    {
                        if (montoAbono >= saldoBase)
                        {
                            // Pago total o con exceso
                            nuevoSaldo = 0;
                            vuelto = Math.Round(montoAbono - saldoBase, 2);
                            guardarBloque = false; // Elimina la deuda del archivo
                        }
                        else
                        {
                            // Pago parcial: actualiza el saldo restante
                            nuevoSaldo = Math.Round(saldoBase - montoAbono, 2);
                            vuelto = 0;
                            bloqueTemporal.Add("SALDO RESTANTE: " + nuevoSaldo);
                        }

                        saldoCalculado = true;
                        encontrado = true;
                    }

                    bloqueTemporal.Add(linea); // Agrega el separador al bloque

                    if (guardarBloque)
                        nuevasLineas.AddRange(bloqueTemporal); // Conserva el bloque en el archivo

                    // Reset para el siguiente bloque
                    guardarBloque = true;
                    bloqueCliente = false;
                    tieneSaldo = false;
                    saldoCalculado = false;
                }
                else
                {
                    bloqueTemporal.Add(linea); // Líneas internas del bloque
                }
            }

            File.WriteAllLines(archivoDeudores, nuevasLineas); // Sobreescribe el archivo con los cambios

            // === RESULTADO EN CONSOLA ===
            if (encontrado)
            {
                Console.WriteLine("==============================");
                Console.WriteLine("=== PAGO REGISTRADO ===");
                Console.WriteLine("CLIENTE      : " + clienteActual);
                Console.WriteLine("MONTO ABONADO: S/ " + montoAbono);
                Console.WriteLine("MÉTODO       : " + metodoPago);
                Console.WriteLine("SALDO FINAL  : S/ " + nuevoSaldo);
                if (vuelto > 0)
                    Console.WriteLine("VUELTO       : S/ " + vuelto);
                Console.WriteLine("==============================");

                // Solo emite comprobante si la deuda fue cancelada completamente
                if (nuevoSaldo == 0)
                {
                    string detalle = "CANCELACION TOTAL DE DEUDA | MONTO: S/ " + saldoBase + " | METODO: " + metodoPago;
                    TIPOCOMPROBANTE(ref clienteActual, detalle, saldoBase, metodoPago, 0, "", saldoBase);
                }
            }
            else
            {
                Console.WriteLine("Cliente no encontrado: " + clienteBuscar);
            }
        }
        public static void BUSCARDEUDA(string archivoDeudores, string nombre)
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            bool encontrado = false; // Si se halló el cliente
            bool imprimir = false;   // Si estamos imprimiendo su bloque

            foreach (string linea in File.ReadLines(archivoDeudores))
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    string cliente = linea.Substring("CLIENTE:".Length).Trim();
                    // Compara ignorando mayúsculas/minúsculas
                    imprimir = cliente.Equals(nombre, StringComparison.OrdinalIgnoreCase);

                    if (imprimir)
                    {
                        Console.WriteLine("=== DEUDOR ENCONTRADO ===");
                        Console.WriteLine(linea); // Imprime la línea CLIENTE:
                        encontrado = true;
                    }
                }
                else if (linea.StartsWith("================"))
                {
                    if (imprimir)
                    {
                        Console.WriteLine("------------------------");
                        imprimir = false; // Deja de imprimir al llegar al separador
                    }
                }
                else if (imprimir)
                {
                    Console.WriteLine(linea); // Imprime los datos internos del bloque
                }
            }

            if (!encontrado)
                Console.WriteLine("No se encontró el cliente: " + nombre);

        }
        public static void IMPRIMIRDEUDOR(
           string cliente, // Nombre del cliente
           string totalFinal, // Monto total de la deuda original
           string aCuenta, // Monto que ya abonó
           string saldo,  // Saldo pendiente por pagar
           string dias,  // Días de crédito otorgados
           string fechaInicio, // Fecha en que inició la deuda
           string fechaFin, // Fecha límite de pago
           string tipocred) // Tipo de crédito (ej: al crédito, cuotas, etc.)
        {
            // Solo imprime el campo si tiene valor, evita líneas vacías
            if (!string.IsNullOrEmpty(cliente)) Console.WriteLine("CLIENTE: " + cliente);
            if (!string.IsNullOrEmpty(totalFinal)) Console.WriteLine("TOTAL FINAL: " + totalFinal);
            if (!string.IsNullOrEmpty(aCuenta)) Console.WriteLine("A CUENTA: " + aCuenta);
            if (!string.IsNullOrEmpty(saldo)) Console.WriteLine("SALDO RESTANTE: " + saldo);
            if (!string.IsNullOrEmpty(dias)) Console.WriteLine("DIAS: " + dias);
            if (!string.IsNullOrEmpty(fechaInicio)) Console.WriteLine("FECHA INICIO: " + fechaInicio);
            if (!string.IsNullOrEmpty(fechaFin)) Console.WriteLine("FECHA FIN: " + fechaFin);
            if (!string.IsNullOrEmpty(tipocred)) Console.WriteLine("TIPO DE CRÉDITO: " + tipocred);

            Console.WriteLine("------------------------");
        }
        public static void VERDEUDORES(string archivoDeudores)
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores);

            // Variables para acumular datos de cada bloque
            string cliente = "", totalFinal = "", aCuenta = "", saldo = "",
                   dias = "", fechaInicio = "", fechaFin = "", tipocred = "";

            Console.WriteLine("===========================");
            Console.WriteLine("========= DEUDORES ========");
            Console.WriteLine("===========================");

            foreach (string linea in lineas)
            {
                // Extrae cada campo del bloque actual
                if (linea.StartsWith("CLIENTE:")) cliente = linea.Substring("CLIENTE:".Length).Trim();
                else if (linea.StartsWith("TOTAL FINAL:")) totalFinal = linea.Substring("TOTAL FINAL:".Length).Trim();
                else if (linea.StartsWith("A CUENTA:")) aCuenta = linea.Substring("A CUENTA:".Length).Trim();
                else if (linea.StartsWith("SALDO RESTANTE:")) saldo = linea.Substring("SALDO RESTANTE:".Length).Trim();
                else if (linea.StartsWith("DIAS PARA PAGAR:")) dias = linea.Substring("DIAS PARA PAGAR:".Length).Trim();
                else if (linea.StartsWith("FECHA DE INICIO:")) fechaInicio = linea.Substring("FECHA DE INICIO:".Length).Trim();
                else if (linea.StartsWith("FECHA DE FIN:")) fechaFin = linea.Substring("FECHA DE FIN:".Length).Trim();
                else if (linea.StartsWith("TIPO DE CRÉDITO:")) tipocred = linea.Substring("TIPO DE CRÉDITO:".Length).Trim();
                else if (linea.StartsWith("================"))
                {
                    // Al llegar al separador, imprime el deudor y resetea
                    IMPRIMIRDEUDOR(cliente, totalFinal, aCuenta, saldo, dias, fechaInicio, fechaFin, tipocred);
                    cliente = totalFinal = aCuenta = saldo = dias = fechaInicio = fechaFin = tipocred = "";
                }
            }
        }
        public static void MENUDEUDAS()
        {
            Console.WriteLine("======================");
            Console.WriteLine("====== OPCIONES ======");
            Console.WriteLine("======================");
            Console.WriteLine("1. Ver todos deudores");
            Console.WriteLine("2. Buscar deudor");
            Console.WriteLine("3. Abonar / Cancelar");
            Console.WriteLine("4. Ver deudas vencidas");
            Console.WriteLine("5. Salir");

            Console.Write("Seleccione opción: ");
            string opcion = Console.ReadLine();
            if (opcion.ToUpper() == "SALIR") return;

            int opcionG;
            // Valida que sea un número entre 1 y 5
            while (!int.TryParse(opcion, out opcionG) || opcionG < 1 || opcionG > 5)
            {
                Console.WriteLine("Ingrese una opción válida (1-5):");
                opcion = Console.ReadLine();
                if (opcion.ToUpper() == "SALIR") return;
            }

            switch (opcionG)
            {
                case 1:
                    VERDEUDORES(archivoDeudores); // Muestra todos los deudores
                    break;
                case 2:
                    Console.Write("INGRESE EL NOMBRE DEL CLIENTE A BUSCAR: ");
                    string nombre = Console.ReadLine();
                    if (nombre.ToUpper() == "SALIR") return;
                    BUSCARDEUDA(archivoDeudores, nombre); // Busca un deudor específico
                    break;
                case 3:
                    ABONARDEUDA(archivoDeudores); // Registra un abono o cancelación
                    break;
                case 4:
                    DEUDASVENCIDAS(archivoDeudores); // Muestra deudas con fecha vencida
                    break;
                case 5:
                    break; // Sale del menú
            }
        }
        //MENU PRINCIPAL
        static void Main(string[] args)
        {
            int opcion = 0;
            CARGARSESION();
            CARGARVENTASDIA(); 
            while (true) // 🔴 SISTEMA SIEMPRE ENCENDIDO
            {
                Console.WriteLine("===============================");
                Console.WriteLine("= SISTEMA DE VENTAS / COMPRAS= ");
                Console.WriteLine("===============================");
                Console.WriteLine("1. Registrar Ventas");
                Console.WriteLine("2. Registrar Compras");
                Console.WriteLine("3. Proveedores");
                Console.WriteLine("4. Finalizar Día");
                Console.WriteLine("5. Gestionar Deudores");
                Console.WriteLine("6. Salir");
                Console.Write("SELECCIONE: ");

                while (!int.TryParse(Console.ReadLine(), out opcion) || opcion < 1 || opcion > 6)
                {
                    Console.WriteLine("Ingrese solo números (1-6)...");
                }

                switch (opcion)
                {
                    case 1:
                        REGISTROVENTAS();
                        break;

                    case 2:
                        MENUPROD();
                        break;
                    case 3:
                        MENUPROV();
                        break;

                    case 4:
                        {
                            bool volverMenu = false;

                            while (!volverMenu)
                            {
                                Console.WriteLine("==============================");
                                Console.WriteLine("===== FINALIZAR DÍA =====");
                                Console.WriteLine("==============================");
                                Console.WriteLine("1. Mostrar estadísticas");
                                Console.WriteLine("2. Mostrar historial de venta total");
                                Console.WriteLine("3. Volver al menú principal");
                                Console.Write("SELECCIONE: ");

                                int OPCIÓNF;

                                while (!int.TryParse(Console.ReadLine(), out OPCIÓNF) || OPCIÓNF < 1 || OPCIÓNF > 3)
                                {
                                    Console.WriteLine("Ingrese 1, 2 o 3");
                                }
                                switch (OPCIÓNF)
                                {
                                    case 1:
                                        ESTADISTICAS();
                                        break;

                                    case 2:
                                        CIERRECAJA();
                                        break;

                                    case 3:
                                        volverMenu = true; // 
                                        break;

                                    default:
                                        Console.WriteLine("Opción inválida");
                                        break;
                                }
                            }

                            break;
                        }

                    case 5:
                        MENUDEUDAS();
                        break;
                    case 6:
                        string OPCF;
                        do
                        {
                            Console.WriteLine("¿Desea finalizar el programa? (SI/NO)");
                            OPCF = Console.ReadLine().ToUpper();
                            if (OPCF == "S" || OPCF == "SI")
                            {
                                Console.WriteLine("Cerrando sistema...");
                                return;
                            }
                            else if (OPCF == "N" || OPCF == "NO")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Ingrese solo SI o NO");
                            }
                        } while (true);
                        break;
                }
            }
        }
    }
}
