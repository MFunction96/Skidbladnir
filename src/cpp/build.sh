#!/bin/sh
# Declare global variant
PROJECT_DIR=`pwd`
THIRD_PARTY_DIR="${PROJECT_DIR}/thirdparty"
STAGE_INFO='\e[1;36m'
NORMAL_INFO='\e[0m'
ERROR_INFO='\e[0;31m'
OK_INFO='\e[0;32m'

Cleanup() {
	# Force to switch in currect working directory.
	echo "${STAGE_INFO}Clean up all previous building files...${NORMAL_INFO}"
	echo "${NORMAL_INFO}Switch working directory into ${PROJECT_DIR}"
	cd "${PROJECT_DIR}"
	if [ -d "bin" ]; then
		echo "${NORMAL_INFO}Clean up bin folder..."
		rm -rf bin
	fi
	if [ -d "build" ]; then
		echo "${NORMAL_INFO}Clean up build folder..."
		rm -rf build
	fi
	if [ -d "lib" ]; then
		echo "${NORMAL_INFO}Clean up lib folder..."
		rm -rf lib
	fi
	if [ -d "thirdparty" ]; then
		echo "${NORMAL_INFO}Clean up thirdparty folder..."
		rm -rf thirdparty
	fi
}

Compile() {
	echo "${STAGE_INFO}Start compiling Kankrelats...${NORMAL_INFO}"
	OPENSSL_VERSION_PREFIX="3.0.5"
	OPENSSL_ARCHIVE_FILE="openssl-${OPENSSL_VERSION_PREFIX}.tar.gz"
	OPENSSL_DOWNLOAD_DIR="${THIRD_PARTY_DIR}/TMP"
	OPENSSL_EXTRACT_DIR="${OPENSSL_DOWNLOAD_DIR}/openssl-openssl-${OPENSSL_VERSION_PREFIX}"
	OPENSSL_ROOT_DIR="${THIRD_PARTY_DIR}/openssl"
	# OpenSSL cross compile
	echo "${STAGE_INFO}Compiling OpenSSL...${NORMAL_INFO}"
	cd "${PROJECT_DIR}"
	mkdir -p "${OPENSSL_DOWNLOAD_DIR}"
	cd "${OPENSSL_DOWNLOAD_DIR}"
	curl -SL "https://github.com/openssl/openssl/archive/refs/tags/${OPENSSL_ARCHIVE_FILE}" -o "${OPENSSL_ARCHIVE_FILE}"
	tar -zxf ${OPENSSL_ARCHIVE_FILE}
	rm ${OPENSSL_ARCHIVE_FILE}
	cd "${OPENSSL_EXTRACT_DIR}"
	if [ $# -eq 1 ] && [ $1 = true ]; then
		./Configure mingw64 shared --prefix="${OPENSSL_ROOT_DIR}" --openssldir="${OPENSSL_ROOT_DIR}/openssl"
	else
		./Configure linux-generic64 shared --prefix="${OPENSSL_ROOT_DIR}" --openssldir="${OPENSSL_ROOT_DIR}/openssl"
	fi
	make depend
	make
	make test
	make install
	cd "${PROJECT_DIR}"
	# Remove temporary files
	rm -rf ${OPENSSL_DOWNLOAD_DIR}
	mkdir -p lib
	if [ $# -eq 1 ] && [ $1 = true ]; then
		cp thirdparty/openssl/bin/libcrypto-3-x64.dll lib/libcrypto-3-x64.dll
		cp thirdparty/openssl/bin/libssl-3-x64.dll lib/libssl-3-x64.dll
	else
		cp thirdparty/openssl/lib/libcrypto.so.3 lib/libcrypto.so.3
		cp thirdparty/openssl/lib/libssl.so.3 lib/libssl.so.3
	fi
	# CMake building
	echo "${STAGE_INFO}Compiling Kankrelats...${NORMAL_INFO}"
	cmake -B./build -DCMAKE_TOOLCHAIN_FILE=./cmake/native.cmake -G Ninja .
	cmake --build ./build --target all -- -j 10
	echo "${OK_INFO}Building finished!${NORMAL_INFO}"
}

CrossCompile() {
	echo "${STAGE_INFO}Start cross compiling Kankrelats...${NORMAL_INFO}"
	OPENSSL_VERSION_PREFIX="1_1_1g"
	OPENSSL_ARCHIVE_FILE="OpenSSL_${OPENSSL_VERSION_PREFIX}.tar.gz"
	OPENSSL_DOWNLOAD_DIR="${THIRD_PARTY_DIR}/openssl-OpenSSL_${OPENSSL_VERSION_PREFIX}"
	OPENSSL_ROOT_DIR="${PROJECT_DIR}/lib/openssl"
	# OpenSSL cross compile
	echo "${STAGE_INFO}Cross compiling OpenSSL...${NORMAL_INFO}"
	cd "${PROJECT_DIR}"
	mkdir -p "${OPENSSL_DOWNLOAD_DIR}"
	cd "${THIRD_PARTY_DIR}"
	curl -SL "https://github.com/openssl/openssl/archive/${OPENSSL_ARCHIVE_FILE}" -o "${OPENSSL_ARCHIVE_FILE}"
	tar -zxf ${OPENSSL_ARCHIVE_FILE}
	rm ${OPENSSL_ARCHIVE_FILE}
	cd "${OPENSSL_DOWNLOAD_DIR}"
	./Configure linux-generic32 shared --prefix="${OPENSSL_ROOT_DIR}" --openssldir="${OPENSSL_ROOT_DIR}/openssl" --cross-compile-prefix="arm-linux-gnueabihf-"
	make depend
	make
	make test
	make install
	# CMake building
	echo "${STAGE_INFO}Cross compiling Kankrelats...${NORMAL_INFO}"
	cd "${PROJECT_DIR}"
	cmake -B./build -DCMAKE_TOOLCHAIN_FILE=./cmake/cross.cmake -G Ninja .
	cmake --build ./build --target all -- -j 10
	echo "${OK_INFO}Building finished!${NORMAL_INFO}"
}

NativeCompile() {
	Compile false
}

MinGWCompile() {
	Compile true
}

ShowHelp() {
	echo "${NORMAL_INFO}Usage: ./build.sh <options...> [--clean]"
	echo "${NORMAL_INFO}	--cross		Cross compile Kankrelats"
	echo "${NORMAL_INFO}	--mingw		Compile Kankrelats on MinGW64"
	echo "${NORMAL_INFO}	--help		This help text"
	echo "${NORMAL_INFO}	--native	Native compile Kankrelats"
}

########################## Main script ##########################
if [ $# -eq 2 ] && [ $2 = "--clean" ]; then
    Cleanup
fi

if [ $1 = "--native" ]; then
    NativeCompile
elif [ $1 = "--cross" ]; then
	CrossCompile
elif [ $1 = "--mingw" ]; then
	MinGWCompile
else
	echo "${ERROR_INFO}Please input valid argument!${NORMAL_INFO}"
	ShowHelp
fi