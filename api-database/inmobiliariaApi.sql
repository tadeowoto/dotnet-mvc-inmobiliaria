-- MySQL dump 10.13  Distrib 8.0.42, for macos15 (arm64)
--
-- Host: 127.0.0.1    Database: inmobiliariamovil
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `contrato`
--

DROP TABLE IF EXISTS `contrato`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contrato` (
  `id_contrato` int NOT NULL AUTO_INCREMENT,
  `id_inquilino` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `monto_alquiler` int NOT NULL,
  `estado` tinyint(1) DEFAULT '1',
  `fecha_inicio` datetime DEFAULT NULL,
  `fecha_finalizacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id_contrato`),
  KEY `id_inquilino` (`id_inquilino`),
  KEY `id_inmueble` (`id_inmueble`),
  CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`),
  CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contrato`
--

LOCK TABLES `contrato` WRITE;
/*!40000 ALTER TABLE `contrato` DISABLE KEYS */;
INSERT INTO `contrato` VALUES (1,3,5,85000,1,'2024-05-01 00:00:00','2026-04-30 00:00:00');
/*!40000 ALTER TABLE `contrato` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmueble`
--

DROP TABLE IF EXISTS `inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmueble` (
  `id_inmueble` int NOT NULL AUTO_INCREMENT,
  `direccion_inmueble` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ambientes_inmueble` int NOT NULL,
  `tipo_inmueble` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `uso_inmueble` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `latitud` decimal(9,6) DEFAULT NULL,
  `longitud` decimal(9,6) DEFAULT NULL,
  `precio_inmueble` double NOT NULL,
  `disponible_inmueble` tinyint(1) NOT NULL DEFAULT '1',
  `tieneContratoVigente` tinyint(1) DEFAULT '0',
  `id_propietario` int NOT NULL,
  `superficie` decimal(8,2) DEFAULT NULL,
  `foto_inmueble` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_inmueble`),
  KEY `id_propietario` (`id_propietario`),
  CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmueble`
--

LOCK TABLES `inmueble` WRITE;
/*!40000 ALTER TABLE `inmueble` DISABLE KEYS */;
INSERT INTO `inmueble` VALUES (1,'Av. Perón 1500, San Luis',5,'Casa','Residencial',-33.310000,-66.340000,310000,1,0,2,180.00,'https://ejemplo.com/fotos/inmueble4.png'),(2,'San Martín 820, San Luis',1,'Oficina','Comercial',-33.301500,-66.335000,85000,1,1,2,40.00,'https://ejemplo.com/fotos/inmueble5.png'),(3,'Ruta 3 y Av. Viento Chorrillero, Juana Koslay',1,'Depósito','Comercial',-33.268000,-66.275000,150000,1,0,2,300.00,'https://ejemplo.com/fotos/inmueble6.png'),(4,'Av. Lafinur 900, San Luis',4,'Casa','Residencial',-33.305000,-66.345000,220000,0,1,2,130.50,'https://ejemplo.com/fotos/inmueble7.png'),(5,'Pringles 750, Villa Mercedes',2,'Local','Comercial',-33.680000,-65.461000,180000,1,0,2,90.00,'https://ejemplo.com/fotos/inmueble8.png'),(6,'Av.Corrientes',3,'','Residencial',-32.110000,32.333300,150000,1,1,2,75.00,'/uploads/inmuebles/d853e5e4-fe66-4336-9262-c4bd6a840ed1.jpg'),(7,'Av.Corrientes',3,'','Residencial',-32.110000,32.333300,150000,0,1,2,75.00,'/uploads/inmuebles/3ee6f0de-7922-4e7e-b964-987906459d28.jpg'),(8,'Av.Los pinos',3,'Casa','Residencial',-32.110000,32.333300,150000,0,1,2,75.00,'/uploads/inmuebles/d8135a77-0c5a-410f-bd66-0721de4c1ab8.jpg'),(9,'Av.Corrientes',3,'Departamento','Residencial',-32.110000,32.333300,150000,0,1,2,75.00,NULL),(10,'Av.Corrientes',3,'Departamento','Residencial',-32.110000,32.333300,150000,0,1,2,75.00,'/uploads/inmuebles/f77be38b-6c20-43f0-afee-580e44e69c6c.jpg'),(11,'Av.Corrientes',3,'Departamento','Residencial',-32.110000,32.333300,150000,0,1,2,75.00,'/uploads/inmuebles/93052cef-e0ba-4c60-9971-4f5b9a245b0f.jpg');
/*!40000 ALTER TABLE `inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilino`
--

DROP TABLE IF EXISTS `inquilino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilino` (
  `id_inquilino` int NOT NULL AUTO_INCREMENT,
  `dni_inquilino` int NOT NULL,
  `nombre_inquilino` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido_inquilino` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `direccion_inquilino` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefono_inquilino` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_inquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilino`
--

LOCK TABLES `inquilino` WRITE;
/*!40000 ALTER TABLE `inquilino` DISABLE KEYS */;
INSERT INTO `inquilino` VALUES (1,40111222,'Ana','García','Rivadavia 300, San Luis','2664558899'),(2,33444555,'Luis','Martínez','Sucre 1050, Villa Mercedes','2657661122'),(3,41555666,'Sofía','López','Lavalle 450, San Luis','2664773344');
/*!40000 ALTER TABLE `inquilino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pago`
--

DROP TABLE IF EXISTS `pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pago` (
  `id_pago` int NOT NULL AUTO_INCREMENT,
  `nro_pago` int NOT NULL,
  `id_alquiler` int NOT NULL,
  `fecha_pago` date NOT NULL,
  `importe_pago` double NOT NULL,
  `detalle` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_pago`),
  KEY `id_alquiler` (`id_alquiler`),
  CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_alquiler`) REFERENCES `contrato` (`id_contrato`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pago`
--

LOCK TABLES `pago` WRITE;
/*!40000 ALTER TABLE `pago` DISABLE KEYS */;
INSERT INTO `pago` VALUES (7,1,1,'2024-06-05',220000,'Pago cuota Junio 2024',1),(8,2,1,'2024-07-05',220000,'Pago cuota Julio 2024',1),(9,3,1,'2024-08-04',220000,'Pago cuota Agosto 2024',1);
/*!40000 ALTER TABLE `pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietario`
--

DROP TABLE IF EXISTS `propietario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietario` (
  `id_propietario` int NOT NULL AUTO_INCREMENT,
  `dni_propietario` int NOT NULL,
  `nombre_propietario` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido_propietario` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email_propietario` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `telefono_propietario` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `clave` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id_propietario`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietario`
--

LOCK TABLES `propietario` WRITE;
/*!40000 ALTER TABLE `propietario` DISABLE KEYS */;
INSERT INTO `propietario` VALUES (1,12345678,'Juan','Pérez','juan@gmail.com','1122334455','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3'),(2,43434343,'Juan','Gomez','propietario@gmail.com','123456789','gX4f2o9+m6pMI+RBEgODyuhW/q0dBXR1ZP8ovZ2dfE0=');
/*!40000 ALTER TABLE `propietario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'inmobiliariamovil'
--

--
-- Dumping routines for database 'inmobiliariamovil'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-03 12:12:51
