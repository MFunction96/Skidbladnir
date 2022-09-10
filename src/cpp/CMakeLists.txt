cmake_minimum_required(VERSION 3.15.0)
project(Skidbladnir LANGUAGES C CXX VERSION 0.1.0)

# Set debug defination
set(CMAKE_C_FLAGS_DEBUG "${CMAKE_C_FLAGS_DEBUG} -D_DEBUG")
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -D_DEBUG")

# Keep independent
set (CMAKE_LIBRARY_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/bin")
set (CMAKE_RUNTIME_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/lib")
set (CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/lib")

set(OPENSSL_ROOT_DIR "${PROJECT_SOURCE_DIR}/thirdparty/openssl")
set(OPENSSL_INCLUDE_DIR "${PROJECT_SOURCE_DIR}/thirdparty/openssl/include")
if(WIN32)
        set(OPENSSL_LIBRARIES "${PROJECT_SOURCE_DIR}/thirdparty/openssl/lib64")
else()
        set(OPENSSL_LIBRARIES "${PROJECT_SOURCE_DIR}/thirdparty/openssl/lib")
endif()


########################## Configure Kankrelats Source ##########################

set (PROJECT_LIBRARY_DIRECTORY "${PROJECT_SOURCE_DIR}/lib")
set (PROJECT_INCLUDE_DIRECTORY "${PROJECT_SOURCE_DIR}/include")

# Find all source files

aux_source_directory("${PROJECT_SOURCE_DIR}/src" PROJECT_SOURCE_DIRECTORY)

########################## Configure Kankrelats Build ##########################

add_library(Kankrelats SHARED
        ${PROJECT_SOURCE_DIRECTORY}
        )

target_include_directories(Kankrelats
        PUBLIC ${Boost_INCLUDE_DIRS}
        PRIVATE ${PROJECT_INCLUDE_DIRECTORY}
        PRIVATE ${OPENSSL_INCLUDE_DIR}
        )

target_link_directories(Kankrelats
        PRIVATE ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}
        PRIVATE ${Boost_LIBRARY_DIRS}
        PRIVATE ${PROJECT_LIBRARY_DIRECTORY}
        PRIVATE ${OPENSSL_LIBRARIES}
        )

if(WIN32)
    target_link_libraries(Kankrelats
            PRIVATE ${Boost_LIBRARIES}
            PRIVATE libcrypto
            PRIVATE wsock32
            PRIVATE ws2_32
            )
else()
    target_link_libraries(Kankrelats
            PRIVATE ${Boost_LIBRARIES}
            PRIVATE crypto
            )
endif()
