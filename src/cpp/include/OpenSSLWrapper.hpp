//
// Created by MFunc on 9/5/2022.
//

#ifndef _OPENSSL_WRAPPER_HPP_
#define _OPENSSL_WRAPPER_HPP_

#include "Wrapper.hpp"
#include <openssl/evp.h>

EXPORT const EVP_MD* EVPGetDigestByName(const char* name);
EXPORT EVP_MD_CTX* EVPMDCTXNew();
EXPORT int EVPDigestInitEx2(EVP_MD_CTX* ctx, const EVP_MD* md);
EXPORT int EVPDigestUpdate(EVP_MD_CTX* ctx, const char* data, size_t length);
EXPORT int EVPDigestFinalEx(EVP_MD_CTX* ctx, unsigned char* hash, unsigned int* length);
EXPORT void EVPMDCTXFree(EVP_MD_CTX* ctx);

#endif //_OPENSSL_WRAPPER_HPP_
