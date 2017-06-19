-- phpMyAdmin SQL Dump
-- version 4.6.3
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jun 20, 2017 at 01:09 AM
-- Server version: 5.5.51-MariaDB
-- PHP Version: 5.6.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Database: `JefBase`
--
CREATE DATABASE IF NOT EXISTS `JefBase` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `JefBase`;

-- --------------------------------------------------------

--
-- Table structure for table `Quotes`
--

CREATE TABLE `Quotes` (
  `ID` bigint(20) UNSIGNED NOT NULL COMMENT 'Index',
  `QUOTE` text COMMENT 'The Quote',
  `SUBMITTER` varchar(120) DEFAULT NULL,
  `CHANNEL` varchar(255) DEFAULT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Quotes`
--
ALTER TABLE `Quotes`
  ADD PRIMARY KEY (`ID`);
ALTER TABLE `Quotes` ADD FULLTEXT KEY `QUOTE` (`QUOTE`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Quotes`
--
ALTER TABLE `Quotes`ALTER TABLE `Quotes`
  MODIFY `ID` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Index';
