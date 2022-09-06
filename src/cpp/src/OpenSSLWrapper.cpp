//
// Created by MFunc on 9/5/2022.
//

#include "OpenSSLWrapper.hpp"

const EVP_MD *EVPGetDigestByName (const char *name)
{
	return EVP_get_digestbyname (name);
}

EVP_MD_CTX *EVPMDCTXNew ()
{
	return EVP_MD_CTX_new ();
}

int EVPDigestInitEx2 (EVP_MD_CTX *ctx, const EVP_MD *md)
{
	return EVP_DigestInit_ex2 (ctx, md, nullptr);
}

int EVPDigestUpdate (EVP_MD_CTX *ctx, const char *data, size_t length)
{
	return EVP_DigestUpdate (ctx, data, length);
}

int EVPDigestFinalEx (EVP_MD_CTX *ctx, unsigned char *hash, unsigned int *length)
{
	return EVP_DigestFinal_ex (ctx, hash, length);
}

void EVPMDCTXFree (EVP_MD_CTX *ctx)
{
	return EVP_MD_CTX_free (ctx);
}
