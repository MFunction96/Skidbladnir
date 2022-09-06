//
// Created by MFunc on 9/6/2022.
//

#ifndef _WRAPPER_HPP_
#define _WRAPPER_HPP_

#if defined(__GNUC__)
#define EXPORT extern "C" __attribute__((visibility("default")))
#elif defined(_MSC_VER)
#define EXPORT extern "C" __declspec(dllexport)
#endif

#include <bits/stdc++.h>

#endif //_WRAPPER_HPP_
