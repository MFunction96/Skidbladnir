#!/bin/bash
# Declare global variant
PROJECT_DIR=`pwd`
THIRD_PARTY_DIR="${PROJECT_DIR}/thirdparty"
STAGE_INFO='\033[0;36m'
RESET_INFO='\033[0m'
ERROR_INFO='\033[0;31m'
OK_INFO='\033[0;32m'
_CLEANUP=0
_COMPILE_RANGE=3
_COMPILE_PLATFORM=0
OPENSSL_VERSION_PREFIX="3.0.5"
OPENSSL_ARCHIVE_FILE="openssl-${OPENSSL_VERSION_PREFIX}.tar.gz"
OPENSSL_DOWNLOAD_DIR="${THIRD_PARTY_DIR}/TMP"
OPENSSL_EXTRACT_DIR="${OPENSSL_DOWNLOAD_DIR}/openssl-openssl-${OPENSSL_VERSION_PREFIX}"
OPENSSL_ROOT_DIR="${THIRD_PARTY_DIR}/openssl"

# Clean all previous building file.
Cleanup() {
	echo -e "${STAGE_INFO}Clean up all previous building files...${RESET_INFO}"
	echo -e "Switch working directory into ${PROJECT_DIR}"
	cd "${PROJECT_DIR}"
	if [ -d "bin" ]; then
		echo -e "Clean up bin folder..."
		rm -rf bin
	fi

	if [ -d "build" ]; then
		echo -e "Clean up build folder..."
		rm -rf build
	fi

	if [ -d "lib" ]; then
		echo -e "Clean up lib folder..."
		rm -rf lib
	fi

	if [ -d "thirdparty" ]; then
		echo -e "Clean up thirdparty folder..."
		rm -rf thirdparty
	fi

	echo -e "${OK_INFO}Completed clean up.${RESET_INFO}"
}

# Compile OpenSSL library
CompileOpenSSL() {
	# OpenSSL cross compile
	echo -e "${STAGE_INFO}Compiling OpenSSL...${RESET_INFO}"
	cd "${PROJECT_DIR}"
	mkdir -p "${OPENSSL_DOWNLOAD_DIR}"
	cd "${OPENSSL_DOWNLOAD_DIR}"
	curl -SL "https://github.com/openssl/openssl/archive/refs/tags/${OPENSSL_ARCHIVE_FILE}" -o "${OPENSSL_ARCHIVE_FILE}" --retry 100 --retry-all-errors
	tar -zxf ${OPENSSL_ARCHIVE_FILE}
	rm ${OPENSSL_ARCHIVE_FILE}
	cd "${OPENSSL_EXTRACT_DIR}"
	if [ $_COMPILE_PLATFORM -eq 3 ]; then
		./Configure mingw64 shared --prefix="${OPENSSL_ROOT_DIR}" --openssldir="${OPENSSL_ROOT_DIR}/openssl"
	elif [ $_COMPILE_PLATFORM -eq 2 ]; then
		./Configure linux-generic32 shared --prefix="${OPENSSL_ROOT_DIR}" --openssldir="${OPENSSL_ROOT_DIR}/openssl" --cross-compile-prefix="arm-linux-gnueabihf-"
	elif [ $_COMPILE_PLATFORM -eq 1 ]; then
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
	if [ $_COMPILE_PLATFORM = "MinGW" ]; then
		cp thirdparty/openssl/bin/libcrypto-3-x64.dll lib/libcrypto-3-x64.dll
		cp thirdparty/openssl/bin/libssl-3-x64.dll lib/libssl-3-x64.dll
	else
		cp thirdparty/openssl/lib/libcrypto.so.3 lib/libcrypto.so.3
		cp thirdparty/openssl/lib/libssl.so.3 lib/libssl.so.3
	fi

	echo -e "${OK_INFO}OpenSSL compilation finished!${RESET_INFO}"
}

# Compile Third Party Library
CompileThirdParty() {
	echo -e "${STAGE_INFO}Compiling third party library...${RESET_INFO}"
	CompileOpenSSL
	echo -e "${OK_INFO}Third party library compilation finished!${RESET_INFO}"
}

# Compile Self project
CompileSelf() {
	echo -e "${STAGE_INFO}Compiling Kankrelats...${RESET_INFO}"
	cd "${PROJECT_DIR}"
	if [ $_COMPILE_PLATFORM -eq 2 ]; then
		cmake -B./build -DCMAKE_TOOLCHAIN_FILE=./cmake/cross.cmake -G Ninja .
	else
		cmake -B./build -DCMAKE_TOOLCHAIN_FILE=./cmake/native.cmake -G Ninja .
	fi

	cmake --build ./build --target all -- -j 10
	echo -e "${OK_INFO}Kankrelats compilation finished!${RESET_INFO}"	
}

# Main compile function
Compile() {
	echo -e "${STAGE_INFO}Start compiling Skidbladnir...${RESET_INFO}"
	if [ $(($_COMPILE_RANGE&1)) -ne 0 ]; then
		CompileThirdParty
	fi

	if [ $(($_COMPILE_RANGE&2)) -ne 0 ]; then
		CompileSelf
	fi

	echo -e "${OK_INFO}Skidbladnir compilation finished!${RESET_INFO}"
}

# Show help page
ShowHelp() {
	echo -e "Usage: ./build.sh <option...> [--nothirdparty] [--noself] [--clean] [--help]"
	echo -e ""
	echo -e "OPTION"
	echo -e "	--cross		Cross compile Skidbladnir"
	echo -e "	--mingw		Compile Skidbladnir on MinGW64"
	echo -e "	--native	Native compile Skidbladnir"
	echo -e "SWITCH"
	echo -e "	--clean		Clean all previous building files"
	echo -e "	--help		Display this help and exit"
	echo -e "	--noself		Build without Skidbladnir"
	echo -e "	--nothirdparty		Build without third party library"
}

########################## Main script ##########################

for arg in "$@" 
do
	if [ $arg = "--native" ] && [ $_COMPILE_PLATFORM -eq 0 ]; then
		_COMPILE_PLATFORM=1
		continue
	elif [ $arg = "--cross" ] && [ $_COMPILE_PLATFORM -eq 0 ]; then
		_COMPILE_PLATFORM=2
		continue
	elif [ $arg = "--mingw" ] && [ $_COMPILE_PLATFORM -eq 0 ]; then
		_COMPILE_PLATFORM=3
		continue
	fi

	if [ $arg = "--clean" ]; then
		_CLEANUP=1
		continue
	fi

	if [ $arg = "--nothirdparty" ]; then
		_COMPILE_RANGE=$(($_COMPILE_RANGE & ~1))
		continue
	elif [ $arg = "--noself" ]; then
		_COMPILE_RANGE=$(($_COMPILE_RANGE & ~2))
		continue
	fi

	echo -e "${ERROR_INFO}$arg is invalid argument!${RESET_INFO}"
	ShowHelp
	exit 1
done

if [ $_COMPILE_RANGE -eq 0 ] || [ $_COMPILE_PLATFORM -eq 0 ] ; then
	echo -e "${ERROR_INFO}$arg are invalid arguments!${RESET_INFO}"
	ShowHelp
	exit 1
fi

if [ $_CLEANUP -eq 1 ]; then
	Cleanup
fi

Compile

exit 0